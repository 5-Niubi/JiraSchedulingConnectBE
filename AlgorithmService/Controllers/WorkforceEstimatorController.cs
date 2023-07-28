using AlgorithmServiceServer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;
using UtilsLibrary.Exceptions;

namespace AlgorithmServiceServer.Controllers

{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class WorkforceEstimatorController : ControllerBase
    {


        private readonly IEstimateWorkforceService estimateWorkforceService;
        public WorkforceEstimatorController(IEstimateWorkforceService estimateWorkforceService, ILogger<WorkforceEstimatorController> logger)
        {
            this.estimateWorkforceService = estimateWorkforceService;
        }

        [HttpGet]
        async public Task<IActionResult> GetEstimateWorkforce(int projectId)
        {
            try
            {
                return Ok(await estimateWorkforceService.Execute(projectId));
            }
            catch (NotSuitableInputException ex)
            {
                var response = new ResponseMessageDTO(ex.Errors);
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