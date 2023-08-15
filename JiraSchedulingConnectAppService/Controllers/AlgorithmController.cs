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
                int maxRetries = 3;

           
                for (int retryCount = 0; retryCount < maxRetries; retryCount++)
                {
                    try
                    {
                        await algorithmService.IsValidExecuteAuthorize();
                        break;
                    }


                    catch (Microsoft.Data.SqlClient.SqlException ex)
                    {
                        if (retryCount < maxRetries - 1)
                        {
                            // Perform a retry after a delay (optional)
                            TimeSpan retryDelay = TimeSpan.FromSeconds(10); // You can adjust the delay as needed
                            await Task.Delay(retryDelay);
                        }
                        else
                        {
                            // Handle the timeout error after retries
                            var response = new ResponseMessageDTO("SQL timeout error");
                            // Set other response properties as needed
                            return StatusCode(500, response);
                        }
                    }
                }

               
                return Ok(algorithmService.ExecuteAlgorithm(parameterId));
            }
            catch (MicroServiceAPIException ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                response.Data = ex.mircoserviceResponse;
                return BadRequest(response);
            }

            
            catch (System.IndexOutOfRangeException ex)
            {
                throw;
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


        [HttpGet]
        public async Task<IActionResult> GetExecuteAlgorithmDailyLimited()
        {
            try
            {
                return Ok(await algorithmService.GetExecuteAlgorithmLimited());
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
