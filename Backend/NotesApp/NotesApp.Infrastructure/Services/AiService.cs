using Microsoft.Extensions.Configuration;
using NotesApp.Application.Services;
using Microsoft.Extensions.Logging;
using NotesApp.Application.DTOs.AI;
using NotesApp.Application.Prompts;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Linq;
using Microsoft.ApplicationInsights;

namespace NotesApp.Infrastructure.Services
{
    public class AiService : IAiService
    {
        private readonly HttpClient _http;
        private readonly string _model;
        private readonly ILogger<AiService> _logger;
        private readonly TelemetryClient _telemetry;

        private static readonly JsonSerializerOptions JsonOptions =
            new() { PropertyNameCaseInsensitive = true };

        public AiService(
            IConfiguration config,
            ILogger<AiService> logger,
            TelemetryClient telemetry)
        {
            _logger = logger;
            _telemetry = telemetry;

            string apiKey =
                config["AzureOpenAI:ApiKey"]
                ?? config["OpenAI:ApiKey"]
                ?? throw new InvalidOperationException("AI API key not configured.");

            _model =
                config["AzureOpenAI:Deployment"]
                ?? config["OpenAI:Model"]
                ?? "gpt-4o-mini";

            string endpoint =
                config["AzureOpenAI:Endpoint"]
                ?? "https://api.openai.com/v1/";

            _http = new HttpClient
            {
                BaseAddress = new Uri(endpoint)
            };

            _http.DefaultRequestHeaders.Add("api-key", apiKey);
            _logger.LogInformation("TelemetryClient injected: {Injected}", _telemetry != null);

        }

        // ===========================
        // Core AI Execution
        // ===========================
        private async Task<AIResponse> RunChatAsync(
            string systemPrompt,
            string userPrompt,
            int maxTokens = 300)
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation(
                "AI request started. Model={Model}, MaxTokens={MaxTokens}",
                _model, maxTokens);

            try
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
                using var content =
                    new StringContent(json, Encoding.UTF8, "application/json");

                using var response = await _http.PostAsync(
                    $"openai/deployments/{_model}/chat/completions?api-version=2024-08-01-preview",
                    content);

                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    _telemetry.GetMetric("AI.Requests.Failed").TrackValue(1);

                    _logger.LogError(
                        "AI request failed. Status={Status}. Body={Body}",
                        response.StatusCode, error);

                    throw new Exception(error);
                }

                using var stream = await response.Content.ReadAsStreamAsync();
                using var doc = await JsonDocument.ParseAsync(stream);

                var choice = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? string.Empty;

                var usage = doc.RootElement.GetProperty("usage");

                int promptTokens = usage.GetProperty("prompt_tokens").GetInt32();
                int completionTokens = usage.GetProperty("completion_tokens").GetInt32();

                _telemetry.GetMetric("AI.Requests.Total").TrackValue(1);
                _telemetry.GetMetric("AI.Tokens.Prompt").TrackValue(promptTokens);
                _telemetry.GetMetric("AI.Tokens.Completion").TrackValue(completionTokens);

                stopwatch.Stop();
                _telemetry.GetMetric("AI.LatencyMs").TrackValue(stopwatch.ElapsedMilliseconds);

                _logger.LogInformation(
                    "AI request completed in {Duration} ms",
                    stopwatch.ElapsedMilliseconds);

                return new AIResponse
                {
                    Output = choice,
                    Model = _model,
                    PromptTokens = promptTokens,
                    CompletionTokens = completionTokens
                };
            }
            catch
            {
                stopwatch.Stop();
                throw;
            }
        }

        // ===========================
        // Public API
        // ===========================
        public Task<AIResponse> SummarizeAsync(SummarizeRequest req)
        {
            string input =
                $"{req.Text}\n\nTone: {req.Tone}\nMaxLength: {req.MaxLength}";

            return RunChatAsync(
                PromptLibrary.Summarize.SystemPrompt,
                input,
                req.MaxLength);
        }

        public Task<AIResponse> RewriteAsync(RewriteRequest req)
        {
            string input = $"{req.Text}\nRewrite style: {req.Style}";

            return RunChatAsync(
                PromptLibrary.Rewrite.SystemPrompt,
                input);
        }

        public async Task<AIResponse> SuggestTagsAsync(SuggestTagsRequest req)
        {
            AIResponse ai = await RunChatAsync(
                PromptLibrary.Tagging.SystemPrompt,
                req.Text);

            try
            {
                var doc = JsonDocument.Parse(ai.Output);
                var tags = doc.RootElement
                    .GetProperty("tags")
                    .EnumerateArray()
                    .Select(x => x.GetString())
                    .Where(x => !string.IsNullOrWhiteSpace(x));

                ai.Output = string.Join(", ", tags);
            }
            catch
            {
                ai.Output = ai.Output
                    .Replace("\n", " ")
                    .Replace("*", "")
                    .Replace("Tags:", "", StringComparison.OrdinalIgnoreCase);
            }

            return ai;
        }

        public Task<AIResponse> GenerateNoteAsync(GenerateNoteRequest req)
        {
            string input =
                $"Topic: {req.Topic}\nFormat: {req.Format}";

            return RunChatAsync(
                PromptLibrary.NoteGeneration.SystemPrompt,
                input,
                250);
        }

        // Simple streaming implementation to satisfy IAiService contract.
        // Currently yields the full response as a single chunk. Replace with real streaming if the API supports it.
        public async IAsyncEnumerable<string> StreamChatAsync(string systemPrompt, string userPrompt)
        {
            var ai = await RunChatAsync(systemPrompt, userPrompt);
            yield return ai.Output;
        }
    }
}
