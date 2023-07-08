using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        public TasksController()
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return NoContent();
        }
    }
}

