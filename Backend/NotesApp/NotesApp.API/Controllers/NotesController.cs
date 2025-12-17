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
        private readonly INoteRepository _noteRepository;
        private const string UserId = "demo-user";

        public NotesController(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
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
            var (items, totalCount) =
                await _noteRepository.GetPagedAsync(
                    page,
                    pageSize,
                    orderBy,
                    direction,
                    UserId);

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
            var note = await _noteRepository.GetByIdAsync(id, UserId);
            if (note == null) return NotFound();

            return Ok(new NoteResponseDto
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                Tags = note.Tags,
                Summary = note.Summary,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt
            });
        }

        // ===========================
        // POST
        // ===========================
        [HttpPost]
        public async Task<IActionResult> Create(CreateNoteDto dto)
        {
            var note = new Notes
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

            await _noteRepository.AddAsync(note);

            return Ok(note);
        }

        // ===========================
        // PUT
        // ===========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UpdateNoteDto dto)
        {
            var existing = await _noteRepository.GetByIdAsync(id, UserId);
            if (existing == null) return NotFound();

            existing.Title = dto.Title;
            existing.Content = dto.Content;
            existing.Tags = dto.Tags;
            existing.Summary = dto.Summary;
            existing.UpdatedAt = DateTime.UtcNow;

            await _noteRepository.UpdateAsync(existing);
            return Ok(existing);
        }

        // ===========================
        // DELETE
        // ===========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _noteRepository.DeleteAsync(id, UserId);
            return NoContent();
        }
    }
}
