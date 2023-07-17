using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;
using ModelLibrary.DTOs.Export;
using System.IO;
using UtilsLibrary.Exceptions;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private readonly IExportService exportService;
        
        public ExportController(IExportService exportService)
        {
            this.exportService = exportService;
        }
        [Authorize]
        [HttpGet]
        async public Task<IActionResult> ExportToJira(int scheduleId)
        {
            try
            {
                var response = await exportService.ToJira(scheduleId);
                return Ok(response);
            }

            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpGet]
        async public Task<IActionResult> ExportToMicrosoftProject(int scheduleId)
        {
            try
            {

                (var fileName , var responseStream) = await exportService.ToMSProject(scheduleId);
                return File(responseStream, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {

                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }


    }
}
