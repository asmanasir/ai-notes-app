namespace NotesApp.Application.DTOs.AI
{
    public class SummarizeRequest
    {
        public string Text { get; set; } = string.Empty;
        public string Tone { get; set; } = "neutral";   // NEW
        public int MaxLength { get; set; } = 150;       // NEW
    }
}
