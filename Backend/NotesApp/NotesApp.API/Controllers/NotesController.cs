using Microsoft.AspNetCore.Mvc;
using NotesApp.Application.Interfaces;
using NotesApp.Domain.Entities;
using NotesApp.Application.DTOs.Notes;

namespace NotesApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly INoteRepository _repo;

        public NotesController(INoteRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NoteResponseDto>>> GetAll()
        {
            var notes = await _repo.GetAllAsync();

            var result = notes.Select(n => new NoteResponseDto
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                Tags = n.Tags,
                Summary = n.Summary,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt
            });

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<NoteResponseDto>> GetById(Guid id)
        {
            var note = await _repo.GetByIdAsync(id);
            if (note == null)
                return NotFound();

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

        [HttpPost]
        public async Task<ActionResult<NoteResponseDto>> Create(CreateNoteDto dto)
        {
            var note = new Notes
            {
                Id = Guid.NewGuid(),
                Title = dto.Title ?? string.Empty,
                Content = dto.Content ?? string.Empty,
                Tags = dto.Tags ?? string.Empty,
                Summary = string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(note);

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

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<NoteResponseDto>> Update(Guid id, UpdateNoteDto dto)
        {
            var note = await _repo.GetByIdAsync(id);
            if (note == null)
                return NotFound();

            note.Title = dto.Title ?? note.Title;
            note.Content = dto.Content ?? note.Content;
            note.Tags = dto.Tags ?? note.Tags;
            note.Summary = dto.Summary ?? note.Summary;
            note.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(note);

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

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _repo.DeleteAsync(id);
            return NoContent();
        }
    }
}
