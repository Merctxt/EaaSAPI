using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EaaSAPI.Models;

namespace EaaSAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SmtpController : ControllerBase
    {
        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SmtpSendRequest request)
        {
            // Implementation for sending email via SMTP
            return Ok();
        }
    }
}
