using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class Policy : ControllerBase
    {
        private readonly ILoggerManager _Logger;

        public Policy(ILoggerManager logger)
        {
            _Logger = logger;
        }

        [HttpGet]
        [Authorize("GetPremiumPlan")]
        public async Task<IActionResult> GetPremiumPlan()
        {
            return Ok();
        }


    }
}
