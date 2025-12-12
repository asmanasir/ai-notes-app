namespace NotesApp.Application.DTOs.AI
{
    public class GenerateNoteRequest
    {
        public string Topic { get; set; } = string.Empty;
        public string Format { get; set; } = "paragraph";   // NEW
    }
}
