using Microsoft.AspNetCore.Mvc;
using UnknownCampServer.Core.Services;

namespace UnknownCampServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TreasureController : ControllerBase
    {
        private readonly ITreasureService _treasureService;

        public TreasureController(ITreasureService treasureService)
        {
            _treasureService = treasureService;
        }

        [HttpPost("add-mock-treasure")]
        public async Task<IActionResult> AddMockTreasure()
        {
            await _treasureService.AddMockTreasureAsync();
            return Ok(new { message = "Mock treasure added successfully." });
        }
    }
}
