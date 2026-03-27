using System.ComponentModel.DataAnnotations;

namespace NotesApp.Application.DTOs.Auth
{
    public class RegisterDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = default!;

        [Required, MinLength(6)]
        public string Password { get; set; } = default!;
    }
}
