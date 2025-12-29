using Microsoft.EntityFrameworkCore;
using NotesApp.Application.Interfaces;
using NotesApp.Domain.Entities;
using NotesApp.Infrastructure.Data;

namespace NotesApp.Infrastructure.Repositories
{
    public class SqlNoteRepository : ISqlNoteRepository
    {
        private readonly NotesDbContext _db;

        public SqlNoteRepository(NotesDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Notes>> GetAllAsync(string userId)
            => await _db.Notes.Where(n => n.UserId == userId).ToListAsync();

        public async Task<Notes?> GetByIdAsync(string id, string userId)
            => await _db.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        public async Task AddAsync(Notes note)
        {
            _db.Notes.Add(note);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Notes note)
        {
            _db.Notes.Update(note);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id, string userId)
        {
            var note = await GetByIdAsync(id, userId);
            if (note != null)
            {
                _db.Notes.Remove(note);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<(IEnumerable<Notes>, int)> GetPagedAsync(
            int page,
            int pageSize,
            string orderBy,
            string direction,
            string userId)
        {
            var query = _db.Notes
                .Where(n => n.UserId == userId);

            // Deterministic ordering (VERY important for paging)
            query = direction.ToLower() == "desc"
                ? query.OrderByDescending(n => n.UpdatedAt)
                : query.OrderBy(n => n.UpdatedAt);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

    }
}
