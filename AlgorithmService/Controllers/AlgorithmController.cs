using AlgorithmServiceServer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;
using UtilsLibrary.Exceptions;

namespace AlgorithmServiceServer.Controllers
{   
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AlgorithmController : ControllerBase
    {
        private readonly IAlgorithmComputeService accessData;
        public AlgorithmController(IAlgorithmComputeService accessData)
        {
            this.accessData = accessData;
        }

        [HttpGet]
        async public Task<IActionResult> Index()
        {
            return NotFound();
        }

        [HttpGet]
        async public Task<IActionResult> ExecuteAlgorithm(int parameterId)
        {
            try
            {
                return Ok(await accessData.GetDataToCompute(parameterId));
            }
            catch (NotFoundException ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }
    }
}