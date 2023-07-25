using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary;
using ModelLibrary.DTOs;
using UtilsLibrary.Exceptions;

namespace JiraSchedulingConnectAppService.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService subsService;
        private readonly ILoggerManager _Logger;

        public SubscriptionController(ISubscriptionService subscService,
            ILoggerManager logger)
        {
            this._Logger = logger;
            this.subsService = subscService;
            
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var subs = await subsService.GetCurrentSubscription();
                return Ok(subs);
            }
            catch (NotFoundException ex)
            {
                this._Logger.LogWarning(ex.Message);
                var response = new ResponseMessageDTO(ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                this._Logger.LogError(ex.Message);
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }
    }
}
