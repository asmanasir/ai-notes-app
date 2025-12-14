using Microsoft.AspNetCore.Mvc;

namespace NotesApp.API.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Test controller works!");
        }
    }
}
