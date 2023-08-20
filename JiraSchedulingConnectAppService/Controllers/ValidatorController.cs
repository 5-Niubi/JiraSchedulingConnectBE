using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ValidatorController : ControllerBase
    {
        private IValidatorService ValidatorService;
        private readonly ILoggerManager _Logger;
        public ValidatorController(IValidatorService validatorService, ILoggerManager logger)

        {
            _Logger = logger;
            ValidatorService = validatorService;
        }

        [HttpGet]
        public async Task<IActionResult> IsDAG(int projectId)
        {
            try
            {
                var response = await ValidatorService.IsValidDAG(projectId);
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

