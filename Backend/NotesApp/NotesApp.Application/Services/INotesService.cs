using NotesApp.Domain.Entities;

namespace NotesApp.Application.Interfaces
{
    public interface INotesService
    {
        Task<(IEnumerable<Notes> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            string orderBy,
            string direction,
            string userId);

        Task<Notes?> GetByIdAsync(string id, string userId);

        Task CreateAsync(Notes note);

        Task UpdateAsync(Notes note);

        Task DeleteAsync(string id, string userId);
    }
}
