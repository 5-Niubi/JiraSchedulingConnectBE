using System;
using JiraSchedulingConnectAppService.Services;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;
using ModelLibrary.DTOs.Projects;
using UtilsLibrary.Exceptions;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ParameterController: ControllerBase
	{


        private IParametersService parametersService;

        private readonly ILoggerService _Logger;
        public ParameterController(IParametersService parametersService, ILoggerService logger )
		{
            this.parametersService = parametersService;
            this._Logger = logger;

        }

        [HttpPost]
        public async Task<IActionResult> SaveParameter([FromBody] ModelLibrary.DTOs.PertSchedule.ParameterRequest parameterRequest)
        {
            try
            {
                var projectCreated = await parametersService.SaveParams(parameterRequest);
                return Ok(projectCreated);
            }
            catch(NotSuitableInputException ex) {
                this._Logger.Log(LogLevel.Error, ex);
                var response = ex.Errors;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                this._Logger.Log(LogLevel.Critical, ex);
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }
    }
}

