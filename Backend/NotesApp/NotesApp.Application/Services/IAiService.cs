using NotesApp.Application.DTOs.AI;

namespace NotesApp.Application.Services
{
    public interface IAiService
    {
        Task<AIResponse> SummarizeAsync(SummarizeRequest req);
        Task<AIResponse> RewriteAsync(RewriteRequest req);
        Task<AIResponse> SuggestTagsAsync(SuggestTagsRequest req);
        Task<AIResponse> GenerateNoteAsync(GenerateNoteRequest req);

        // NEW: Streaming
        IAsyncEnumerable<string> StreamChatAsync(string systemPrompt, string userPrompt);
    }
}
