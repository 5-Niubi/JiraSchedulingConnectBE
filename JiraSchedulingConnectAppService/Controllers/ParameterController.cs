using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;
using ModelLibrary.DTOs.Parameters;
using UtilsLibrary.Exceptions;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ParameterController : ControllerBase
    {


        private IParametersService parametersService;
        private readonly ILoggerManager _Logger;

        public ParameterController(IParametersService parametersService, ILoggerManager logger)

        {
            _Logger = logger;
            this.parametersService = parametersService;


        }

        [HttpGet]
        public async Task<IActionResult> GetWorkforceParameter(string? id)
        {
            try
            {
                var response = await parametersService.GetWorkforceParameter(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex.Message);
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveParameter([FromBody] ParameterRequestDTO parameterRequest)
        {
            try
            {

                var projectCreated = await parametersService.SaveParams(parameterRequest);

                return Ok(projectCreated);
            }


            catch (UnAuthorizedException ex)
            {
                _Logger.LogWarning(ex.Message);
                var response = ex.Errors;
                return StatusCode(412, response);
            }

            catch (NotSuitableInputException ex)
            {
                _Logger.LogWarning(ex.Message);
                var response = ex.Errors;
                return BadRequest(response);
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

