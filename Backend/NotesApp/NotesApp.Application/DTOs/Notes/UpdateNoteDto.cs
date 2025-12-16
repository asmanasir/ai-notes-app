namespace NotesApp.Application.DTOs.Notes
{
    public class UpdateNoteDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
    }
}
