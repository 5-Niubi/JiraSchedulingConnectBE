using System;
using JiraSchedulingConnectAppService.Services;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;
using ModelLibrary.DTOs.Invalidator;
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
        

        public ParameterController(IParametersService parametersService)
		{
            this.parametersService = parametersService;

        }

        [HttpPost]
        public async Task<IActionResult> SaveParameter([FromBody] ModelLibrary.DTOs.PertSchedule.ParameterRequest parameterRequest)
        {
            try
            {
                var projectCreated = await parametersService.SaveParams(parameterRequest);
                return Ok(projectCreated);
            }
            catch(ParamInputFailureException ex) {
                var response = ex.Errors;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }
    }
}

