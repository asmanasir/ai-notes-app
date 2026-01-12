using Microsoft.AspNetCore.Mvc;
using NotesApp.Application.DTOs.Common;
using NotesApp.Application.DTOs.Notes;
using NotesApp.Application.Interfaces;
using NotesApp.Domain.Entities;

namespace NotesApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly INotesService _notesService;
        private readonly ILogger<NotesController> _logger;

        private const string UserId = "demo-user";

        public NotesController(
            INotesService notesService,
            ILogger<NotesController> logger)
        {
            _notesService = notesService;
            _logger = logger;
        }

        // ===========================
        // GET (paged)
        // ===========================
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<NoteResponseDto>>> GetPaged(
            int page = 1,
            int pageSize = 10,
            string orderBy = "updatedAt",
            string direction = "desc")
        {
            _logger.LogInformation($"GetPaged requested. Page={page}, PageSize={pageSize}, OrderBy={orderBy}, Direction={direction}, User={UserId}");

            var (items, totalCount) =
                await _notesService.GetPagedAsync(
                    page,
                    pageSize,
                    orderBy,
                    direction,
                    UserId);

            _logger.LogInformation($"GetPaged completed. ReturnedCount={items.Count()}, TotalCount={totalCount}, User={UserId}");

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
                    CreatedAt = n.CreatedAt,
                    UpdatedAt = n.UpdatedAt
                }).ToList()
            });
        }

        // ===========================
        // GET by id
        // ===========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            _logger.LogInformation($"GetById requested. NoteId={id}, User={UserId}");

            var note = await _notesService.GetByIdAsync(id, UserId);

            if (note == null)
            {
                _logger.LogWarning($"GetById not found. NoteId={id}, User={UserId}");

                return NotFound();
            }

            _logger.LogInformation($"GetById completed. NoteId={id}, User={UserId}");

            return Ok(note);
        }

        // ===========================
        // POST
        // ===========================
        [HttpPost]
        public async Task<IActionResult> Create(CreateNoteDto dto)
        {
            _logger.LogInformation($"Create requested. TitleLength={dto.Title.Length}, ContentLength={dto.Content.Length}, TagsCount={dto.Tags.Count}, User={UserId}");

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

            try
            {
                await _notesService.CreateAsync(note);

                _logger.LogInformation($"Create completed. NoteId={note.Id}, User={UserId}");

                return Ok(note);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Create failed. User={UserId}");

                throw;
            }
        }

        // ===========================
        // PUT
        // ===========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UpdateNoteDto dto)
        {
            _logger.LogInformation($"Update requested. NoteId={id}, User={UserId}");

            var existing = await _notesService.GetByIdAsync(id, UserId);

            if (existing == null)
            {
                _logger.LogWarning($"Update not found. NoteId={id}, User={UserId}");

                return NotFound();
            }

            existing.Title = dto.Title;
            existing.Content = dto.Content;
            existing.Tags = dto.Tags;
            existing.Summary = dto.Summary;
            existing.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _notesService.UpdateAsync(existing);

                _logger.LogInformation($"Update completed. NoteId={id}, User={UserId}");

                return Ok(existing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Update failed. NoteId={id}, User={UserId}");

                throw;
            }
        }

        // ===========================
        // DELETE
        // ===========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.LogInformation($"Delete requested. NoteId={id}, User={UserId}");

            try
            {
                await _notesService.DeleteAsync(id, UserId);

                _logger.LogInformation($"Delete completed. NoteId={id}, User={UserId}");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Delete failed. NoteId={id}, User={UserId}");

                throw;
            }
        }
    }
}
