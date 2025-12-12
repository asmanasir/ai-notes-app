using Microsoft.Extensions.Configuration;
using NotesApp.Application.DTOs.AI;
using NotesApp.Application.Prompts;
using NotesApp.Application.Services;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NotesApp.Infrastructure.Services
{
    public class AiService : IAiService
    {
        private readonly HttpClient _http;
        private readonly string _model;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public AiService(IConfiguration config)
        {
            // Grab key (Azure or OpenAI)
            string apiKey = config["OpenAI:ApiKey"] ?? config["AzureOpenAI:ApiKey"]
                ?? throw new InvalidOperationException("AI API key not configured.");

            // Prefer Azure model if provided
            _model = config["AzureOpenAI:Deployment"]
                  ?? config["OpenAI:Model"]
                  ?? "gpt-4o-mini";

            // Set correct Azure endpoint
            string endpoint = config["AzureOpenAI:Endpoint"]
                ?? "https://api.openai.com/v1/";

            _http = new HttpClient
            {
                BaseAddress = new Uri(endpoint)
            };

            // Azure uses api-key header, not Bearer tokens
            _http.DefaultRequestHeaders.Add("api-key", apiKey);
        }

        // ------------------------------
        // 🔥 Core reusable method
        // ------------------------------
        private async Task<AIResponse> RunChatAsync(string systemPrompt, string userPrompt, int maxTokens = 300)
        {
            var payload = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                max_tokens = maxTokens,
                temperature = 0.5
            };

            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _http.PostAsync(
                "openai/deployments/" + _model + "/chat/completions?api-version=2024-08-01-preview",
                content
            );

            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"AI Error {response.StatusCode}: {errorBody}");
            }

            using var body = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(body);

            string resultText =
                doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()
                ?? string.Empty;

            var usage = doc.RootElement.GetProperty("usage");

            return new AIResponse
            {
                Output = resultText,
                Model = _model,
                PromptTokens = usage.GetProperty("prompt_tokens").GetInt32(),
                CompletionTokens = usage.GetProperty("completion_tokens").GetInt32()
            };
        }

        // ------------------------------
        // Public methods (using DTOs)
        // ------------------------------

        public Task<AIResponse> SummarizeAsync(SummarizeRequest req)
        {
            string styledPrompt = $"{req.Text}\n\nTone: {req.Tone}\nMaxLength: {req.MaxLength}";
            return RunChatAsync(PromptLibrary.Summarize.SystemPrompt, styledPrompt, req.MaxLength);
        }

        public Task<AIResponse> RewriteAsync(RewriteRequest req)
        {
            string input = $"{req.Text}\nRewrite style: {req.Style}";
            return RunChatAsync(PromptLibrary.Rewrite.SystemPrompt, input);
        }

        public async Task<AIResponse> SuggestTagsAsync(SuggestTagsRequest req)
        {
            // Run the AI model and get a structured result
            AIResponse ai = await RunChatAsync(
                PromptLibrary.Tagging.SystemPrompt,
                req.Text);

            string raw = ai.Output; // ← FIX: AIResponse.Output instead of string

            // Try to parse proper JSON
            try
            {
                var doc = JsonDocument.Parse(raw);
                var tags = doc.RootElement.GetProperty("tags")
                                          .EnumerateArray()
                                          .Select(x => x.GetString() ?? "")
                                          .Where(x => !string.IsNullOrWhiteSpace(x))
                                          .ToList();

                return new AIResponse
                {
                    Output = string.Join(", ", tags),
                    Model = _model,
                    CompletionTokens = ai.CompletionTokens,
                    PromptTokens = ai.PromptTokens
                };
            }
            catch
            {
                // Fallback: try to clean text manually
                var cleaned = raw
                    .Replace("\n", " ")
                    .Replace("*", "")
                    .Replace("Tags:", "")
                    .Replace("tags:", "");

                var tags = cleaned.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                  .Select(t => t.Trim().ToLower())
                                  .ToList();

                return new AIResponse
                {
                    Output = string.Join(", ", tags),
                    Model = _model,
                    CompletionTokens = ai.CompletionTokens,
                    PromptTokens = ai.PromptTokens
                };
            }
        }



        public Task<AIResponse> GenerateNoteAsync(GenerateNoteRequest req)
        {
            string input = $"Topic: {req.Topic}\nFormat: {req.Format}";
            return RunChatAsync(PromptLibrary.NoteGeneration.SystemPrompt, input, 250);
        }

        public async IAsyncEnumerable<string> StreamChatAsync(string systemPrompt, string userPrompt)
        {
            var payload = new
            {
                model = _model,
                messages = new[]
                {
            new { role = "system", content = systemPrompt },
            new { role = "user", content = userPrompt }
        },
                stream = true
            };

            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"openai/deployments/{_model}/chat/completions?api-version=2024-08-01-preview"
            );

            request.Content = content;

            var response = await _http.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead
            );

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            string? line;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (!line.StartsWith("data: "))
                    continue;

                string jsonChunk = line.Substring("data: ".Length);

                if (jsonChunk == "[DONE]")
                    yield break;

                string? contentChunk = null;

                try
                {
                    var doc = JsonDocument.Parse(jsonChunk);
                    contentChunk =
                        doc.RootElement
                            .GetProperty("choices")[0]
                            .GetProperty("delta")
                            .GetProperty("content")
                            .GetString();
                }
                catch
                {
                    // Ignore parsing errors
                }

                if (!string.IsNullOrWhiteSpace(contentChunk))
                    yield return contentChunk; 
            }
        }

    }
}
