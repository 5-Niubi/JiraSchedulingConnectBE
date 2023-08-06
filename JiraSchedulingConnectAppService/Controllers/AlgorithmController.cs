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

    public class AlgorithmController : ControllerBase
    {
        private readonly IAlgorithmService algorithmService;
        private readonly ILoggerManager _Logger;

        public AlgorithmController(IAlgorithmService algorithmService, ILoggerManager logger)
        {
            _Logger = logger;
            this.algorithmService = algorithmService;
        }

        [HttpGet]

        public async Task<IActionResult> ExecuteAlgorithm(int parameterId)
        {
            try
            {
                await algorithmService.IsValidExecuteAuthorize();
                return Ok(algorithmService.ExecuteAlgorithm(parameterId));
            }
            catch (MicroServiceAPIException ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                response.Data = ex.mircoserviceResponse;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex.Message);
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEstimateWorkforce(int projectId)
        {
            try

            {

                return Ok(await algorithmService.EstimateWorkforce(projectId));
            }
            catch (NotSuitableInputException ex)
            {
                _Logger.LogError(ex.Message);
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex.Message);
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEstimateOverallWorkforce(int projectId)
        {
            try
            {
                return Ok(await algorithmService.GetEstimateOverallWorkforce(projectId));
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
