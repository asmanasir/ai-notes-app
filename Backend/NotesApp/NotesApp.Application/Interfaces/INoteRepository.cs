using NotesApp.Domain.Entities;

namespace NotesApp.Application.Interfaces
{
    public interface INoteRepository
    {
        Task<IEnumerable<Notes>> GetAllAsync(string userId);
        Task<Notes?> GetByIdAsync(string id, string userId);
        Task AddAsync(Notes note);
        Task UpdateAsync(Notes note);
        Task DeleteAsync(string id, string userId);

        Task<(IEnumerable<Notes> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            string orderBy,
            string direction,
            string userId
        );
    }
}
