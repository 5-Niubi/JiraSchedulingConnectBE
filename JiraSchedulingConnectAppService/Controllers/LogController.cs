       using System;
using JiraSchedulingConnectAppService.Services;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class LogController: ControllerBase
	{
        private readonly ILoggerService LoggerService;

        public LogController(ILoggerService logger)
		{

            LoggerService = logger;

        }


        [HttpGet]
        async public Task<IActionResult> Get()
        {
            try
            {


                LoggerService.Log(
                    LogLevel.Debug,
                    new Exception("HELLOO"));
                return Ok("OKE");
            }
            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }
    }
}

