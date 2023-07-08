using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WorkforcesController : ControllerBase
    {
        private IWorkforcesService WorkforcesService;
        public WorkforcesController(IWorkforcesService workforcesService)
        {
            this.WorkforcesService = workforcesService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllWorkforces()
        {
            try
            {
                var response = await WorkforcesService.GetAllWorkforces();
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkforce([FromBody] WorkforceDTO.Request workforce)
        {
            try
            {
                return Ok(await WorkforcesService.CreateWorkforce(workforce));
            }
            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkforceById(string id)
        {
            var response = await WorkforcesService.GetWorkforceById(id);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkforce(string id) {
            try
            {
                var w = WorkforcesService.GetWorkforceById(id);
                if (w == null)
                {
                    return BadRequest("Cannot found this workforce!");
                }
                WorkforcesService.DeleteWorkforce(id);
                return Ok("Delete success");
            }
            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpPut("id")]
        public async Task<IActionResult> UpdateWorkforce(string id, WorkforceDTO.Request workforce)
        {
            try
            {
                var w1 = WorkforcesService.GetWorkforceById(id);
                if (w1 == null)
                {
                    return BadRequest("Cannot found this workforce!");
                }
                await WorkforcesService.UpdateWorkforce(workforce);
                return Ok("Update success");
            }
            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }
    }
}
