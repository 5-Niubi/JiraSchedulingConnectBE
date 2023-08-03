using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;
using UtilsLibrary.Exceptions;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private readonly IExportService exportService;
        private readonly ModelLibrary.ILoggerManager _Logger;
        public ExportController(IExportService exportService, ModelLibrary.ILoggerManager logger)
        {
            this._Logger = logger;
            this.exportService = exportService;
        }
        [Authorize]
        [HttpGet]
        async public Task<IActionResult> ExportToJira(int scheduleId, string projectKey, string projectName)
        {
            try
            {
                var response = await exportService.ToJira(scheduleId, projectKey, projectName);
                return Ok(response);
            }

            catch (Exception ex)
            {
                _Logger.LogError(ex.Message);
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpGet]
        async public Task<IActionResult> ExportToMicrosoftProject(int scheduleId, string token)
        {
            try
            {
                (var fileName, var responseStream) = await exportService.ToMSProject(scheduleId, token);
                return File(responseStream, "application/octet-stream", fileName);
            }
            catch (UnAuthorizedException ex)
            {
                _Logger.LogWarning(ex.Message);
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex.Message);
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpGet]
        async public Task<IActionResult> CreateJiraRequest()
        {
            try
            {
                var response = await exportService.JiraRequest(null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex.Message);
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }
    }
}
