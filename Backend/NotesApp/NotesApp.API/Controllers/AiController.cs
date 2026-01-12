using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NotesApp.Application.Services;
using NotesApp.Application.DTOs.AI;

namespace NotesApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly IAiService _ai;
        private readonly ILogger<AiController> _logger;

        public AiController(
            IAiService aiService,
            ILogger<AiController> logger)
        {
            _ai = aiService;
            _logger = logger;
        }

        // ---------------------------
        // SUMMARIZE
        // ---------------------------
        [HttpPost("summarize")]
        public async Task<IActionResult> Summarize([FromBody] SummarizeRequest req)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "Invalid Summarize request. ErrorCount={ErrorCount}",
                    ModelState.ErrorCount);

                return ValidationProblem(ModelState);
            }

            _logger.LogInformation(
                "AI Summarize requested. TextLength={TextLength}, Tone={Tone}, MaxLength={MaxLength}",
                req.Text.Length,
                req.Tone,
                req.MaxLength);

            try
            {
                var result = await _ai.SummarizeAsync(req);

                _logger.LogInformation(
                    "AI Summarize completed. Model={Model}, PromptTokens={PromptTokens}, CompletionTokens={CompletionTokens}",
                    result.Model,
                    result.PromptTokens,
                    result.CompletionTokens);

                return Ok(new { result = result.Output });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI Summarize failed");
                throw;
            }
        }

        // ---------------------------
        // REWRITE
        // ---------------------------
        [HttpPost("rewrite")]
        public async Task<IActionResult> Rewrite([FromBody] RewriteRequest req)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "Invalid Rewrite request. ErrorCount={ErrorCount}",
                    ModelState.ErrorCount);

                return ValidationProblem(ModelState);
            }

            _logger.LogInformation(
                "AI Rewrite requested. TextLength={TextLength}, Style={Style}",
                req.Text.Length,
                req.Style);

            try
            {
                var result = await _ai.RewriteAsync(req);

                _logger.LogInformation(
                    "AI Rewrite completed. Model={Model}, PromptTokens={PromptTokens}, CompletionTokens={CompletionTokens}",
                    result.Model,
                    result.PromptTokens,
                    result.CompletionTokens);

                return Ok(new { result = result.Output });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI Rewrite failed");
                throw;
            }
        }

        // ---------------------------
        // SUGGEST TAGS
        // ---------------------------
        [HttpPost("suggest-tags")]
        public async Task<IActionResult> SuggestTags([FromBody] SuggestTagsRequest req)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "Invalid SuggestTags request. ErrorCount={ErrorCount}",
                    ModelState.ErrorCount);

                return ValidationProblem(ModelState);
            }

            _logger.LogInformation(
                "AI SuggestTags requested. TextLength={TextLength}",
                req.Text.Length);

            try
            {
                var result = await _ai.SuggestTagsAsync(req);

                _logger.LogInformation(
                    "AI SuggestTags completed. Model={Model}, PromptTokens={PromptTokens}, CompletionTokens={CompletionTokens}",
                    result.Model,
                    result.PromptTokens,
                    result.CompletionTokens);

                return Ok(new { result = result.Output });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI SuggestTags failed");
                throw;
            }
        }

        // ---------------------------
        // GENERATE NOTE
        // ---------------------------
        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] GenerateNoteRequest req)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "Invalid GenerateNote request. ErrorCount={ErrorCount}",
                    ModelState.ErrorCount);

                return ValidationProblem(ModelState);
            }

            _logger.LogInformation(
                "AI GenerateNote requested. TopicLength={TopicLength}, Format={Format}",
                req.Topic.Length,
                req.Format);

            try
            {
                var result = await _ai.GenerateNoteAsync(req);

                _logger.LogInformation(
                    "AI GenerateNote completed. Model={Model}, PromptTokens={PromptTokens}, CompletionTokens={CompletionTokens}",
                    result.Model,
                    result.PromptTokens,
                    result.CompletionTokens);

                return Ok(new { result = result.Output });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI GenerateNote failed");
                throw;
            }
        }
    }
}
