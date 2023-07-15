using JiraSchedulingConnectAppService.Services;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public ThreadController(IThreadService threadService) { this.threadService = threadService; }

        [HttpGet]
        public  IActionResult GetThreadResult(string threadId)
        {
            try
            {
                return Ok(threadService.GetThreadResult(threadId));
            }

            catch (NotFoundException ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }
    }
}
