using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary;

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
            this._Logger = logger;
        }

        [HttpGet]
        [Authorize("GetPremiumPlan")]
        public async Task<IActionResult> GetPremiumPlan()
        {
            return Ok();
        }


    }
}
