using NotesApp.Application.Interfaces;
using NotesApp.Domain.Entities;

namespace NotesApp.Infrastructure.Services
{
    public class NoOpRagService : IRagService
    {
        public Task IndexNoteAsync(Note note) => Task.CompletedTask;
        public Task DeleteNoteAsync(string noteId) => Task.CompletedTask;
        public Task<string> ChatAsync(string userId, string question)
            => Task.FromResult("RAG is not configured. Add AzureSearch settings to enable chat with your notes.");
    }
}
