using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesApp.Application.DTOs.Common;
using NotesApp.Application.DTOs.Notes;
using NotesApp.Application.Interfaces;
using NotesApp.Domain.Entities;
using System.Security.Claims;

namespace NotesApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly INotesService _notesService;
        private readonly IRagService _rag;
        private readonly ILogger<NotesController> _logger;

        // Falls back to demo-user when auth is not configured
        private string UserId =>
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "demo-user";

        public NotesController(
            INotesService notesService,
            IRagService rag,
            ILogger<NotesController> logger)
        {
            _notesService = notesService;
            _rag = rag;
            _logger = logger;
        }

        // GET (paged)
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<NoteResponseDto>>> GetPaged(
            int page = 1,
            int pageSize = 10,
            string orderBy = "updatedAt",
            string direction = "desc",
            string? search = null)
        {
            _logger.LogInformation("GetPaged. Page={Page}, PageSize={PageSize}, Search={Search}, User={UserId}", page, pageSize, search, UserId);

            var (items, totalCount) =
                await _notesService.GetPagedAsync(page, pageSize, orderBy, direction, UserId, search);

            return Ok(new PagedResultDto<NoteResponseDto>
            {
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items.Select(n => new NoteResponseDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    Tags = n.Tags,
                    Summary = n.Summary,
                    Pinned = n.Pinned,
                    CreatedAt = n.CreatedAt,
                    UpdatedAt = n.UpdatedAt
                }).ToList()
            });
        }

        // GET by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var note = await _notesService.GetByIdAsync(id, UserId);
            if (note == null) return NotFound();
            return Ok(note);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Create(CreateNoteDto dto)
        {
            var note = new Note
            {
                Id = Guid.NewGuid().ToString(),
                Title = dto.Title,
                Content = dto.Content,
                Tags = dto.Tags,
                Summary = dto.Summary,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = UserId
            };

            await _notesService.CreateAsync(note);
            _ = _rag.IndexNoteAsync(note);  // fire-and-forget

            _logger.LogInformation("Created note {NoteId} for user {UserId}", note.Id, UserId);
            return Ok(note);
        }

        // PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UpdateNoteDto dto)
        {
            var existing = await _notesService.GetByIdAsync(id, UserId);
            if (existing == null) return NotFound();

            existing.Title = dto.Title;
            existing.Content = dto.Content;
            existing.Tags = dto.Tags;
            existing.Summary = dto.Summary;
            existing.Pinned = dto.Pinned;
            existing.UpdatedAt = DateTime.UtcNow;

            await _notesService.UpdateAsync(existing);
            _ = _rag.IndexNoteAsync(existing);  // fire-and-forget

            _logger.LogInformation("Updated note {NoteId} for user {UserId}", id, UserId);
            return Ok(existing);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _notesService.DeleteAsync(id, UserId);
            _ = _rag.DeleteNoteAsync(id);  // fire-and-forget

            _logger.LogInformation("Deleted note {NoteId} for user {UserId}", id, UserId);
            return NoContent();
        }

        // PATCH pin
        [HttpPatch("{id}/pin")]
        public async Task<IActionResult> TogglePin(string id)
        {
            var existing = await _notesService.GetByIdAsync(id, UserId);
            if (existing == null) return NotFound();

            existing.Pinned = !existing.Pinned;
            existing.UpdatedAt = DateTime.UtcNow;

            await _notesService.UpdateAsync(existing);
            return Ok(new { existing.Id, existing.Pinned });
        }
    }
}
