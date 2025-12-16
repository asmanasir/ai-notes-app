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

        public NotesController(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        // ✅ PAGED GET (MAIN LIST)
        // GET: api/notes?page=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<NoteResponseDto>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string orderBy = "updatedAt",
            [FromQuery] string direction = "desc")
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0 || pageSize > 50) pageSize = 10;

            var (items, totalCount) =
                await _noteRepository.GetPagedAsync(page, pageSize, orderBy, direction);

            var response = new PagedResultDto<NoteResponseDto>
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
            };

            return Ok(response);
        }

        // GET: api/notes/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var note = await _noteRepository.GetByIdAsync(id);
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

        // POST: api/notes
        [HttpPost]
        public async Task<IActionResult> Create(CreateNoteDto dto)
        {
            var note = new Notes
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Content = dto.Content,
                Tags = dto.Tags,
                Summary = dto.Summary,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _noteRepository.AddAsync(note);

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

        // PUT: api/notes/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateNoteDto dto)
        {
            var existing = await _noteRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Title = dto.Title;
            existing.Content = dto.Content;
            existing.Tags = dto.Tags;
            existing.Summary = dto.Summary;
            existing.UpdatedAt = DateTime.UtcNow;

            await _noteRepository.UpdateAsync(existing);

            return Ok(existing);
        }

        // DELETE: api/notes/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _noteRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
