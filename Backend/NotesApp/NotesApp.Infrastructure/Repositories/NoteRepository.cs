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
                .AsNoTracking()
                .OrderByDescending(n => n.UpdatedAt)
                .ToListAsync();
        }

        public async Task<Notes?> GetByIdAsync(Guid id)
        {
            return await _context.Notes
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.Id == id);
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
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
                return;

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
        }
    }
}
