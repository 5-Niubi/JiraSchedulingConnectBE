using AlgorithmServiceServer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;
using AlgorithmServiceServer.Services.Interfaces;

namespace AlgorithmServiceServer.Controllers

{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class WorkforceEstimatorController : ControllerBase
    {


        private readonly IEstimateWorkforceService estimateWorkforceService;
        private readonly ILogger<WorkforceEstimatorController> _logger;
        public WorkforceEstimatorController(IEstimateWorkforceService estimateWorkforceService, ILogger<WorkforceEstimatorController> logger)
        {
            this.estimateWorkforceService = estimateWorkforceService;
            _logger = logger;
        }

        [HttpGet]
        async public Task<IActionResult> GetEstimateWorkforce(int projectId)
        {
            try
            {

                return Ok(await estimateWorkforceService.Execute(projectId));

            }
            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);

                return BadRequest(response);
            }
        }



    }
}