using AlgorithmServiceServer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;

namespace AlgorithmServiceServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AlgorithmController : ControllerBase
    {
        private readonly IAccessDataToComputeService accessData;
        public AlgorithmController(IAccessDataToComputeService accessData)
        {
            this.accessData = accessData;
        }
        [HttpGet]
        async public Task<IActionResult> GetTestConverter(int projectId)
        {
            try
            {
                return Ok(await accessData.GetDataToCompute(projectId));

            }
            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }
    }
}