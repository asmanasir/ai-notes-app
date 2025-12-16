using Microsoft.EntityFrameworkCore;
using NotesApp.Application.Interfaces;
using NotesApp.Domain.Entities;
using NotesApp.Infrastructure.Data;

namespace NotesApp.Infrastructure.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly AppDbContext _context;

        public NoteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notes>> GetAllAsync()
        {
            return await _context.Notes
                .OrderByDescending(n => n.UpdatedAt)
                .ToListAsync();
        }

        public async Task<Notes?> GetByIdAsync(Guid id)
        {
            return await _context.Notes.FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task AddAsync(Notes note)
        {
            await _context.Notes.AddAsync(note);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Notes note)
        {
            _context.Notes.Update(note);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var note = await GetByIdAsync(id);
            if (note != null)
            {
                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();
            }
        }

        // ✅ PAGINATION METHOD (NON-GENERIC)
        public async Task<(IEnumerable<Notes> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            string orderBy,
            string direction)
        {
            IQueryable<Notes> query = _context.Notes;

            query = orderBy.ToLower() switch
            {
                "createdat" => direction == "asc"
                    ? query.OrderBy(n => n.CreatedAt)
                    : query.OrderByDescending(n => n.CreatedAt),

                _ => direction == "asc"
                    ? query.OrderBy(n => n.UpdatedAt)
                    : query.OrderByDescending(n => n.UpdatedAt)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
