using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using NotesApp.Application.Interfaces;
using NotesApp.Domain.Entities;

namespace NotesApp.Application.Services
{
    public class NotesService : INotesService
    {
        private readonly INoteRepository _repo;
        private readonly ILogger<NotesService> _logger;
        private readonly TelemetryClient _telemetry;

        public NotesService(
            INoteRepository repo,
            ILogger<NotesService> logger,
            TelemetryClient telemetry)
        {
            _repo = repo;
            _logger = logger;
            _telemetry = telemetry;
        }

        public async Task<(IEnumerable<Note>, int)> GetPagedAsync(
            int page,
            int pageSize,
            string orderBy,
            string direction,
            string userId)
        {
            _logger.LogInformation(
                "Fetching notes page={Page}, size={Size}, user={UserId}",
                page, pageSize, userId);

            _telemetry.GetMetric("Notes.Read").TrackValue(1);

            return await _repo.GetPagedAsync(
                page, pageSize, orderBy, direction, userId);
        }

        public async Task<Note?> GetByIdAsync(string id, string userId)
        {
            _logger.LogInformation(
                "Fetching note {NoteId} for user {UserId}",
                id, userId);

            _telemetry.GetMetric("Notes.Read").TrackValue(1);

            return await _repo.GetByIdAsync(id, userId);
        }

        public async Task CreateAsync(Note note)
        {
            _logger.LogInformation(
                "Creating note {NoteId} for user {UserId}",
                note.Id, note.UserId);

            await _repo.CreateAsync(note);

            _telemetry.GetMetric("Notes.Created").TrackValue(1);
        }

        public async Task UpdateAsync(Note note)
        {
            _logger.LogInformation(
                "Updating note {NoteId} for user {UserId}",
                note.Id, note.UserId);

            await _repo.UpdateAsync(note);

            _telemetry.GetMetric("Notes.Updated").TrackValue(1);
        }

        public async Task DeleteAsync(string id, string userId)
        {
            _logger.LogInformation(
                "Deleting note {NoteId} for user {UserId}",
                id, userId);

            await _repo.DeleteAsync(id, userId);

            _telemetry.GetMetric("Notes.Deleted").TrackValue(1);
        }
    }
}
