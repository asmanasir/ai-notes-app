using NotesApp.Domain.Entities;

namespace NotesApp.Application.Interfaces
{
    public interface INotesService
    {
        Task<(IEnumerable<Note> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            string orderBy,
            string direction,
            string userId);

        Task<Note?> GetByIdAsync(string id, string userId);

        Task CreateAsync(Note note);

        Task UpdateAsync(Note note);

        Task DeleteAsync(string id, string userId);
    }
}
