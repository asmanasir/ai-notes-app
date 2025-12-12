using Microsoft.AspNetCore.Mvc;
using NotesApp.Application.Interfaces;
using NotesApp.Domain.Entities;

namespace NotesApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INoteRepository _noteRepository;

        public NotesController(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notes>>> GetAll()
        {
            var notes = await _noteRepository.GetAllAsync();
            return Ok(notes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Notes>> Get(Guid id)
        {
            var note = await _noteRepository.GetByIdAsync(id);
            if (note == null)
                return NotFound();

            return Ok(note);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Notes note)
        {
            note.Id = Guid.NewGuid();
            note.CreatedAt = DateTime.UtcNow;
            note.UpdatedAt = DateTime.UtcNow;

            await _noteRepository.AddAsync(note);
            return Ok(note);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, Notes updatedNote)
        {
            var existing = await _noteRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            existing.Title = updatedNote.Title;
            existing.Content = updatedNote.Content;
            existing.Tags = updatedNote.Tags;
            existing.UpdatedAt = DateTime.UtcNow;

            await _noteRepository.UpdateAsync(existing);
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _noteRepository.DeleteAsync(id);
            return Ok();
        }
    }
}
