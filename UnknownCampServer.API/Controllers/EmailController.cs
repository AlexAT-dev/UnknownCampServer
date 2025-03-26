using Microsoft.AspNetCore.Mvc;
using UnknownCampServer.Core.Models;
using UnknownCampServer.Core.Services;
using UnknownCampServer.Infrastructure.Services;

namespace UnknownCampServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IAccountService _accountService;

        public EmailController(IEmailService emailService, IAccountService accountService)
        {
            _emailService = emailService;
            _accountService = accountService;
        }

        [HttpPost("SendTo")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.To) ||
                string.IsNullOrWhiteSpace(request.Subject) || string.IsNullOrWhiteSpace(request.Body))
            {
                return BadRequest(new { message = "Invalid request data." });
            }

            var result = await _emailService.SendEmailAsync(request);

            if (result)
                return Ok(new { message = "Email sent successfully!" });

            return BadRequest(new { message = "Failed to send email." });
        }

        [HttpPost("SendToAll")]
        public async Task<IActionResult> SendEmailToAll([FromBody] EmailRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Subject) || string.IsNullOrWhiteSpace(request.Body))
            {
                return BadRequest(new { message = "Invalid request data." });
            }

            var allEmails = await _accountService.GetAllEmailsAsync();
            if (allEmails == null || !allEmails.Any())
            {
                return BadRequest(new { message = "No emails found." });
            }

            foreach (var email in allEmails)
            {
                var emailRequest = new EmailRequest
                {
                    To = email,
                    Subject = request.Subject,
                    Body = request.Body
                };

                var result = await _emailService.SendEmailAsync(emailRequest);
                if (!result)
                {
                    return BadRequest(new { message = $"Failed to send email to {email}" });
                }
            }

            return Ok(new { message = "Email sent successfully to all users!" });
        }
    }
}
