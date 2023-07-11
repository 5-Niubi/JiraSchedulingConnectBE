using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;
using System.Dynamic;
using UtilsLibrary.Exceptions;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AlgorithmController : ControllerBase
    {
        private readonly IAlgorithmService algorithmService;

        public AlgorithmController(IAlgorithmService algorithmService)
        {
            this.algorithmService = algorithmService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTestConverter( int parameterId)
        {
            try
            {              
                return Ok(algorithmService.TestConverter(parameterId));
            }
            catch (MicroServiceAPIException ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                response.Data = ex.mircoserviceResponse;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
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
            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }
    }
}
