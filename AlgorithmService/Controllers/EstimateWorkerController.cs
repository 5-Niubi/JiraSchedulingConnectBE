using AlgorithmServiceServer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;

namespace AlgorithmServiceServer.Controllers

{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class WorkforceEstimatorController : ControllerBase
    {


        private readonly IEstimateWorkerService estimateWorkerService;
        public WorkforceEstimatorController(IEstimateWorkerService estimateWorkerService)
        {
            this.estimateWorkerService = estimateWorkerService;

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