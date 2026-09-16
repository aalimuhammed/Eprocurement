using EPROCUREMENT.DTO;
using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EPROCUREMENT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendMicosoftEmailController : ControllerBase
    {
        private readonly IGraphEmailService _graphEmailService;

        public SendMicosoftEmailController(IGraphEmailService graphEmailService)
        {
            _graphEmailService = graphEmailService;
        }
        [HttpPost("SendEmail")]
        public async Task<IActionResult> SendMicrosoftEmail([FromBody] SendMicrosoftEmailRquest request)
        {
            var result = await _graphEmailService.SendEmailAsync(
                request.RecipientEmail,
                request.Subject,
                request.Body
            );

            if (!result)
            {
                return BadRequest("Email was not sent.");
            }

            return Ok("Email sent successfully.");
        }
    }
}

