namespace NotesApp.Application.DTOs.AI
{
    public class AIResponse
    {
        public string Output { get; set; } = string.Empty;       // AI text result
        public string Model { get; set; } = string.Empty;
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
    }
}
