using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;
using UtilsLibrary.Exceptions;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ThreadController : ControllerBase
    {
        private readonly IThreadService threadService;
        private readonly ModelLibrary.ILoggerManager _Logger;
        public ThreadController(IThreadService threadService, ModelLibrary.ILoggerManager logger)

        {
            this._Logger = logger;
            this.threadService = threadService;

        }

        [HttpGet]
        public IActionResult GetThreadResult(string threadId)
        {
            try
            {

                return Ok(threadService.GetThreadResult(threadId));
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
