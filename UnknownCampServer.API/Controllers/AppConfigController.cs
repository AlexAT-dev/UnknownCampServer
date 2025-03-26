using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UnknownCampServer.Infrastructure.Config;

namespace UnknownCampServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppConfigController : ControllerBase
    {
        private readonly AppConfig _appConfig;

        public AppConfigController(IOptions<AppConfig> appConfig)
        {
            _appConfig = appConfig.Value;
        }


        [HttpGet("ServerVersion")]
        public ActionResult<string> GetServerVersion()
        {
            return Ok(_appConfig.ServerVersion);
        }

        [HttpGet("GameVersion")]
        public ActionResult<string> GetGameVersion()
        {
            return Ok(_appConfig.GameVersion);
        }

        [HttpGet("ResourcesVersion")]
        public ActionResult<string> GetResourcesVersion()
        {
            return Ok(_appConfig.ResourcesVersion);
        }
    }
}
