namespace NotesApp.Application.DTOs.AI
{
    public class RewriteRequest
    {
        public string Text { get; set; } = string.Empty;
        public string Style { get; set; } = "clear";    // NEW
    }
}
