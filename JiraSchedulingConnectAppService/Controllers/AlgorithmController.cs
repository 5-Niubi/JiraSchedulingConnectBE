using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AlgorithmController : ControllerBase
    {
        private readonly IAlgorithmService algorithmService;
        public AlgorithmController(IAlgorithmService algorithmService)
        {
            this.algorithmService = algorithmService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTestConverter(int projectId, int parameterId)
        {
            try
            {
                return Ok(await algorithmService.TestConverter(projectId, parameterId));
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
