using Microsoft.AspNetCore.Mvc;
using NotesApp.Application.Services;
using NotesApp.Application.DTOs.AI;

namespace NotesApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly IAiService _ai;

        public AiController(IAiService aiService)
        {
            _ai = aiService;
        }

        [HttpPost("summarize")]
        public async Task<IActionResult> Summarize([FromBody] SummarizeRequest req)
        {
            var result = await _ai.SummarizeAsync(req);
            return Ok(new { result = result.Output });
        }

        [HttpPost("rewrite")]
        public async Task<IActionResult> Rewrite([FromBody] RewriteRequest req)
        {
            var result = await _ai.RewriteAsync(req);
            return Ok(new { result = result.Output });
        }

        [HttpPost("suggest-tags")]
        public async Task<IActionResult> SuggestTags([FromBody] SuggestTagsRequest req)
        {
            var result = await _ai.SuggestTagsAsync(req);
            return Ok(new { result = result.Output });
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] GenerateNoteRequest req)
        {
            var result = await _ai.GenerateNoteAsync(req);
            return Ok(new { result = result.Output });
        }
    }
}
