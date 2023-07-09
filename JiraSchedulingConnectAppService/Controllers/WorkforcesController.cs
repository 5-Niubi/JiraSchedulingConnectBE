using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class WorkforcesController : ControllerBase
    {
        private IWorkforcesService workforcesService;
        public WorkforcesController(IWorkforcesService workforcesService)
        {
            this.workforcesService = workforcesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWorkforces()
        {
            try
            {
                var response = await workforcesService.GetAllWorkforces();
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
                return Ok(await workforcesService.CreateWorkforce(workforce));
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
            var response = await workforcesService.GetWorkforceById(id);
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
                var w = workforcesService.GetWorkforceById(id);
                if (w == null)
                {
                    return BadRequest("Cannot found this workforce!");
                }
                await workforcesService.DeleteWorkforce(id);
                return Ok("Delete success");
            }
            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateWorkforce(string id, [FromBody] WorkforceDTO.Request workforce)
        {
            try
            {
                var w1 = workforcesService.GetWorkforceById(id);
                if (w1 == null)
                {
                    return BadRequest("Cannot found this workforce!");
                }
                await workforcesService.UpdateWorkforce(workforce);
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
