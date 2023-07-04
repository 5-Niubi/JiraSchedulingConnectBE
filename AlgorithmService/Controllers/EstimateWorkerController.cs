using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;
using AlgorithmServiceServer.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace AlgorithmServiceServer.Controllers

{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WorkforceEstimatorController : ControllerBase
    {


        private readonly IEstimateWorkerService estimateWorkerService;
        private readonly ILogger<WeatherForecastController> _logger;
        public WorkforceEstimatorController(IEstimateWorkerService estimateWorkerService, ILogger<WeatherForecastController> logger)
        {
            this.estimateWorkerService = estimateWorkerService;
            _logger = logger;
        }

        

      

        [HttpGet]
        async public Task<IActionResult> GetEstimateWorkforce(int projectId)
        {
            try
            {
                
                return Ok(await estimateWorkerService.Execute(projectId));

            }
            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);

                return BadRequest(response);
            }
        }


       
    }
}