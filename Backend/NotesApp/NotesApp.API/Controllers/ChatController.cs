using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesApp.Application.DTOs.AI;
using NotesApp.Application.Interfaces;
using System.Security.Claims;

namespace NotesApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IRagService _rag;
        private readonly ILogger<ChatController> _logger;

        private string UserId =>
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "demo-user";

        public ChatController(IRagService rag, ILogger<ChatController> logger)
        {
            _rag = rag;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Question))
                return BadRequest("Question is required.");

            _logger.LogInformation("Chat requested by user {UserId}", UserId);

            var answer = await _rag.ChatAsync(UserId, req.Question);
            return Ok(new { answer });
        }
    }
}
