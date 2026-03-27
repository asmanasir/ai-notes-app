using NotesApp.Domain.Entities;

namespace NotesApp.Application.Interfaces
{
    public interface IRagService
    {
        Task IndexNoteAsync(Note note);
        Task DeleteNoteAsync(string noteId);
        Task<string> ChatAsync(string userId, string question);
    }
}
