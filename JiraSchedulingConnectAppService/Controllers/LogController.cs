using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class LogController : ControllerBase
    {
        private readonly ILoggerManager _Logger;
        public LogController(ILoggerManager logger)
        {
            _Logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _Logger.LogDebug("Debug message");
            _Logger.LogError("Error message");
            _Logger.LogWarning("Warning message");
            _Logger.LogInfo("Information message");

            return Ok();
        }

    }
}

