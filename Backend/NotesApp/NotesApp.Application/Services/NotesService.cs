using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using NotesApp.Application.Interfaces;
using NotesApp.Domain.Entities;

namespace NotesApp.Application.Services
{
    public class NotesService : INotesService
    {
        private readonly ISqlNoteRepository _cosmosRepo;
        private readonly ISqlNoteRepository _sqlRepo;
        private readonly IFeatureManager _featureManager;
        private readonly ILogger<NotesService> _logger;


        public NotesService(
            ISqlNoteRepository cosmosRepo,
            ISqlNoteRepository sqlRepo,
            IFeatureManager featureManager,
            ILogger<NotesService> logger)
        {
            _cosmosRepo = cosmosRepo;
            _sqlRepo = sqlRepo;
            _featureManager = featureManager;
            _logger = logger;
        }

        // ✅ Notice: returns INoteRepository (common contract)
        private async Task<INoteRepository> ResolveRepoAsync()
        {
            var isSql = await _featureManager.IsEnabledAsync("UseAzureSql");

            _logger.LogInformation("UseAzureSql flag = {Flag}", isSql);

            return isSql
                ? _sqlRepo
                : _cosmosRepo;
        }

        public async Task<(IEnumerable<Notes>, int)> GetPagedAsync(
            int page,
            int pageSize,
            string orderBy,
            string direction,
            string userId)
        {
            _logger.LogInformation("Fetching notes for user {UserId}", userId);
            var repo = await ResolveRepoAsync();
            return await repo.GetPagedAsync(page, pageSize, orderBy, direction, userId);
        }

        public async Task<Notes?> GetByIdAsync(string id, string userId)
        {
            var repo = await ResolveRepoAsync();
            return await repo.GetByIdAsync(id, userId);
        }

        public async Task CreateAsync(Notes note)
        {
            var repo = await ResolveRepoAsync();
            await repo.AddAsync(note);
        }

        public async Task UpdateAsync(Notes note)
        {
            var repo = await ResolveRepoAsync();
            await repo.UpdateAsync(note);
        }

        public async Task DeleteAsync(string id, string userId)
        {
            var repo = await ResolveRepoAsync();
            await repo.DeleteAsync(id, userId);
        }
    }
}
