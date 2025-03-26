using Microsoft.AspNetCore.Mvc;
using UnknownCampServer.Core.Services;
using System.Threading.Tasks;

namespace UnknownCampServer.API.Controllers
{
    [Route("[controller]")] 
    [ApiController]
    public class VerificationController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public VerificationController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is required.");
            }

            bool isVerified = await _accountService.VerifyEmailAsync(token);

            if (!isVerified)
            {
                return NotFound("Invalid or expired token.");
            }

            return Ok("Email verified successfully.");
        }
    }
}

