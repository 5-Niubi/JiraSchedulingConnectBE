﻿using JiraSchedulingConnectAppService.Services.Interfaces;
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

        public ExportController(IExportService exportService)

        {
            this.exportService = exportService;
        }
        [Authorize]
        [HttpGet]
        async public Task<IActionResult> ExportToJira(int scheduleId, int projectId)
        {
            try
            {
                var response = await exportService.ToJira(scheduleId, projectId);
                return Ok(response);
            }

            catch (Exception ex)
            {
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
                return Unauthorized();
            }
            catch (Exception ex)
            {
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
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }
    }
}
