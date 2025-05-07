using Microsoft.AspNetCore.Mvc;

namespace babbly_like_service.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { status = "healthy", service = "babbly-like-service" });
        }
    }
} 