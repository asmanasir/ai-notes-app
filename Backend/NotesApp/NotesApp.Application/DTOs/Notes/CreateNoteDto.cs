namespace NotesApp.Application.DTOs.Notes
{
    public class CreateNoteDto
    {
        public string Title { get; set; } = default!;
        public string Content { get; set; } = default!;
        public List<string> Tags { get; set; } = new();
        public string? Summary { get; set; }
    }
}
