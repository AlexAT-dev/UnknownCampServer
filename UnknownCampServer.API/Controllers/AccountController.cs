using Microsoft.AspNetCore.Mvc;
using UnknownCampServer.Core.DTOs;
using UnknownCampServer.Core.Entities;
using UnknownCampServer.Core.Services;

namespace UnknownCampServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccount(string id)
        {
            try
            {
                Account account = await _accountService.GetAccount(id);
                return Ok(account);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        [HttpPost("Create")]
        public async Task<IActionResult> CreateAccount([FromBody] AccountRegDTO accountDTO)
        {
            if (accountDTO == null)
            {
                return BadRequest(new { Message = "Account data is null" } );
            }

            try
            {
                await _accountService.CreateAccountAsync(accountDTO);
                return Ok("Account created successfully. Please check your email to verify.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] AccountLoginDTO loginDto)
        {
            try
            {
                var account = await _accountService.LoginAsync(loginDto);
                return Ok(account);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
        }


        [HttpPost("{id}/OpenTreasure/{treasureId}")]
        public async Task<IActionResult> OpenTreasure(string id, string treasureId)
        {
            try
            {
                var treasureResult = await _accountService.OpenTreasureAsync(id, treasureId);
                return Ok(treasureResult);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("{id}/BuyMatchBox")]
        public async Task<IActionResult> BuyMatchBox(string id)
        {
            try
            {
                var result = await _accountService.BuyMatchBoxAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("{id}/AddMatches/{matches}")] //todo: replace it with more safe function later
        public async Task<IActionResult> AddMatches(string id, int matches)
        {
            try
            {
                var result = await _accountService.AddMatchesAsync(id, matches);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
