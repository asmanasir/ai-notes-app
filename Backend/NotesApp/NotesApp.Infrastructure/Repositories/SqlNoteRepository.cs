using Microsoft.EntityFrameworkCore;
using NotesApp.Application.Interfaces;
using NotesApp.Domain.Entities;
using NotesApp.Infrastructure.Data;

namespace NotesApp.Infrastructure.Repositories
{
    public class SqlNoteRepository : INoteRepository
    {
        private readonly NotesDbContext _db;

        public SqlNoteRepository(NotesDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Notes>> GetAllAsync(string userId)
            => await _db.Notes
                .Where(n => n.UserId == userId)
                .ToListAsync();

        public async Task<Notes?> GetByIdAsync(string id, string userId)
            => await _db.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        // ✅ FIXED: method name matches interface
        public async Task CreateAsync(Notes note)
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
            if (note is null)
                return;

            _db.Notes.Remove(note);
            await _db.SaveChangesAsync();
        }

        public async Task<(IEnumerable<Notes> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            string orderBy,
            string direction,
            string userId)
        {
            var query = _db.Notes
                .Where(n => n.UserId == userId);

            query = direction.Equals("desc", StringComparison.OrdinalIgnoreCase)
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