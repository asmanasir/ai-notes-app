using System.ComponentModel.DataAnnotations;

namespace NotesApp.Application.DTOs.AI
{
    public class SummarizeRequest
    {
        [Required]
        [MinLength(10, ErrorMessage = "Text must be at least 10 characters.")]
        public string Text { get; set; } = string.Empty;

        public string Tone { get; set; } = "neutral";

        [Range(1, 4000, ErrorMessage = "MaxLength must be between 1 and 4000.")]
        public int MaxLength { get; set; } = 150;
    }
}
