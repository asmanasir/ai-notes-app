using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NotesApp.Application.Interfaces;
using NotesApp.Domain.Entities;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace NotesApp.Infrastructure.Services
{
    public class RagService : IRagService
    {
        private readonly HttpClient _http;
        private readonly string _embeddingModel;
        private readonly SearchClient _searchClient;
        private readonly string _gptModel;
        private readonly ILogger<RagService> _logger;

        private const string IndexName = "notes-index";
        private const int VectorDimensions = 1536;

        private static readonly JsonSerializerOptions JsonOptions =
            new() { PropertyNameCaseInsensitive = true };

        public RagService(IConfiguration config, ILogger<RagService> logger)
        {
            _logger = logger;

            var apiKey = config["AzureOpenAI:ApiKey"]
                ?? throw new InvalidOperationException("AzureOpenAI:ApiKey not configured.");
            var endpoint = config["AzureOpenAI:Endpoint"]
                ?? throw new InvalidOperationException("AzureOpenAI:Endpoint not configured.");

            _embeddingModel = config["AzureOpenAI:EmbeddingDeployment"] ?? "text-embedding-ada-002";
            _gptModel = config["AzureOpenAI:Deployment"] ?? "gpt-4o-mini";

            _http = new HttpClient { BaseAddress = new Uri(endpoint) };
            _http.DefaultRequestHeaders.Add("api-key", apiKey);

            var searchEndpoint = config["AzureSearch:Endpoint"]
                ?? throw new InvalidOperationException("AzureSearch:Endpoint not configured.");
            var searchKey = config["AzureSearch:ApiKey"]
                ?? throw new InvalidOperationException("AzureSearch:ApiKey not configured.");

            _searchClient = new SearchClient(
                new Uri(searchEndpoint),
                IndexName,
                new AzureKeyCredential(searchKey));

            EnsureIndexExists(searchEndpoint, searchKey);
        }

        // ===========================
        // Public API
        // ===========================

        public async Task IndexNoteAsync(Note note)
        {
            try
            {
                var text = BuildIndexText(note);
                var embedding = await GetEmbeddingAsync(text);

                var doc = new SearchDocument
                {
                    ["id"] = note.Id,
                    ["userId"] = note.UserId,
                    ["title"] = note.Title,
                    ["content"] = StripHtml(note.Content),
                    ["tags"] = string.Join(", ", note.Tags),
                    ["contentVector"] = embedding
                };

                await _searchClient.MergeOrUploadDocumentsAsync(new[] { doc });
                _logger.LogInformation("Indexed note {NoteId} for user {UserId}", note.Id, note.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to index note {NoteId}", note.Id);
            }
        }

        public async Task DeleteNoteAsync(string noteId)
        {
            try
            {
                await _searchClient.DeleteDocumentsAsync("id", new[] { noteId });
                _logger.LogInformation("Deleted note {NoteId} from search index", noteId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete note {NoteId} from index", noteId);
            }
        }

        public async Task<string> ChatAsync(string userId, string question)
        {
            // 1. Embed the question
            var queryEmbedding = await GetEmbeddingAsync(question);

            // 2. Vector search — filtered to this user's notes
            var searchOptions = new SearchOptions
            {
                Filter = $"userId eq '{userId}'",
                Size = 4,
                VectorSearch = new VectorSearchOptions
                {
                    Queries =
                    {
                        new VectorizedQuery(queryEmbedding)
                        {
                            KNearestNeighborsCount = 4,
                            Fields = { "contentVector" }
                        }
                    }
                },
                Select = { "title", "content", "tags" }
            };

            var results = await _searchClient.SearchAsync<SearchDocument>(null, searchOptions);

            // 3. Build context from retrieved notes
            var contextParts = new List<string>();
            await foreach (var result in results.Value.GetResultsAsync())
            {
                var title = result.Document["title"]?.ToString() ?? "";
                var content = result.Document["content"]?.ToString() ?? "";
                var tags = result.Document["tags"]?.ToString() ?? "";
                contextParts.Add($"Note: {title}\nContent: {content}\nTags: {tags}");
            }

            if (contextParts.Count == 0)
                return "I couldn't find any relevant notes to answer your question.";

            var context = string.Join("\n\n---\n\n", contextParts);

            // 4. Call GPT with context
            var systemPrompt = "You are a helpful assistant that answers questions based only on the user's personal notes provided below. Be concise and cite which note the information comes from.";
            var userPrompt = $"Notes:\n{context}\n\nQuestion: {question}";

            return await RunChatAsync(systemPrompt, userPrompt);
        }

        // ===========================
        // Private helpers
        // ===========================

        private async Task<ReadOnlyMemory<float>> GetEmbeddingAsync(string text)
        {
            var payload = JsonSerializer.Serialize(new { input = text });
            using var content = new StringContent(payload, Encoding.UTF8, "application/json");

            using var response = await _http.PostAsync(
                $"openai/deployments/{_embeddingModel}/embeddings?api-version=2024-08-01-preview",
                content);

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            var values = doc.RootElement
                .GetProperty("data")[0]
                .GetProperty("embedding")
                .EnumerateArray()
                .Select(x => x.GetSingle())
                .ToArray();

            return new ReadOnlyMemory<float>(values);
        }

        private async Task<string> RunChatAsync(string systemPrompt, string userPrompt)
        {
            var payload = new
            {
                model = _gptModel,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                max_tokens = 500,
                temperature = 0.3
            };

            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _http.PostAsync(
                $"openai/deployments/{_gptModel}/chat/completions?api-version=2024-08-01-preview",
                content);

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "";
        }

        private static string BuildIndexText(Note note)
        {
            var sb = new StringBuilder();
            sb.Append(note.Title).Append(' ');
            sb.Append(StripHtml(note.Content)).Append(' ');
            sb.Append(string.Join(' ', note.Tags));
            return sb.ToString();
        }

        private static string StripHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return "";
            var text = Regex.Replace(html, "<[^>]+>", " ");
            text = System.Net.WebUtility.HtmlDecode(text);
            return Regex.Replace(text, @"\s+", " ").Trim();
        }

        private static void EnsureIndexExists(string endpoint, string apiKey)
        {
            try
            {
                var indexClient = new SearchIndexClient(
                    new Uri(endpoint),
                    new AzureKeyCredential(apiKey));

                var fields = new List<SearchField>
                {
                    new SimpleField("id", SearchFieldDataType.String) { IsKey = true },
                    new SimpleField("userId", SearchFieldDataType.String) { IsFilterable = true },
                    new SearchableField("title"),
                    new SearchableField("content"),
                    new SearchableField("tags"),
                    new VectorSearchField("contentVector", VectorDimensions, "notes-vector-profile")
                };

                var index = new SearchIndex(IndexName, fields)
                {
                    VectorSearch = new VectorSearch
                    {
                        Profiles = { new VectorSearchProfile("notes-vector-profile", "notes-hnsw") },
                        Algorithms = { new HnswAlgorithmConfiguration("notes-hnsw") }
                    }
                };

                indexClient.CreateOrUpdateIndex(index);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RagService] Index setup warning: {ex.Message}");
            }
        }
    }
}
