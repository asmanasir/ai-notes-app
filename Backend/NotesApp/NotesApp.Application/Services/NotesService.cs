using Microsoft.Extensions.Logging;
using NotesApp.Application.Interfaces;
using NotesApp.Domain.Entities;

namespace NotesApp.Application.Services
{
    public class NotesService : INotesService
    {
        private readonly INoteRepository _repo;
        private readonly ILogger<NotesService> _logger;

        public NotesService(
            INoteRepository repo,
            ILogger<NotesService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<(IEnumerable<Notes>, int)> GetPagedAsync(
            int page,
            int pageSize,
            string orderBy,
            string direction,
            string userId)
        {
            _logger.LogInformation(
                "Fetching paged notes for user {UserId} (Page={Page}, Size={Size})",
                userId, page, pageSize);

            return await _repo.GetPagedAsync(
                page,
                pageSize,
                orderBy,
                direction,
                userId);
        }

        public async Task<Notes?> GetByIdAsync(string id, string userId)
        {
            _logger.LogInformation(
                "Fetching note {NoteId} for user {UserId}",
                id, userId);

            return await _repo.GetByIdAsync(id, userId);
        }

        public async Task CreateAsync(Notes note)
        {
            _logger.LogInformation(
                "Creating note {NoteId} for user {UserId}",
                note.Id, note.UserId);

            await _repo.CreateAsync(note);
        }

        public async Task UpdateAsync(Notes note)
        {
            _logger.LogInformation(
                "Updating note {NoteId} for user {UserId}",
                note.Id, note.UserId);

            await _repo.UpdateAsync(note);
        }

        public async Task DeleteAsync(string id, string userId)
        {
            _logger.LogInformation(
                "Deleting note {NoteId} for user {UserId}",
                id, userId);

            await _repo.DeleteAsync(id, userId);
        }
    }
}
