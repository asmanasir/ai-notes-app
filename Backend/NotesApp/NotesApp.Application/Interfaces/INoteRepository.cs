using NotesApp.Domain.Entities;

namespace NotesApp.Application.Interfaces
{
    public interface INoteRepository
    {
        Task<IEnumerable<Notes>> GetAllAsync();
        Task<Notes?> GetByIdAsync(Guid id);
        Task AddAsync(Notes note);
        Task UpdateAsync(Notes note);
        Task DeleteAsync(Guid id);
    }
}
