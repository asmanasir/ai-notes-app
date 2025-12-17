namespace NotesApp.Application.DTOs.Notes
{
    public class NoteResponseDto
    {
        public string Id { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Content { get; set; } = default!;
        public List<string> Tags { get; set; } = new();
        public string? Summary { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
