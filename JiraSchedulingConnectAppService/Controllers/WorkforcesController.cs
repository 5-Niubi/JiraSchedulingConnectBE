using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;
using ModelLibrary.DTOs.Parameters;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize]
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
        public async Task<IActionResult> CreateWorkforce([FromBody] WorkforceDTORequest workforce)
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

        [HttpGet]
        public async Task<IActionResult> GetWorkforceById(string id)
        {
            try
            {
                var response = await workforcesService.GetWorkforceById(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteWorkforce(string id) {
            try
            {
                var w = workforcesService.GetWorkforceById(id);
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
        public async Task<IActionResult> UpdateWorkforce(string id, [FromBody] WorkforceDTORequest workforce)
        {
            try
            {
                var w1 = workforcesService.GetWorkforceById(id);
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
