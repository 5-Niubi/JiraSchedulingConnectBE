using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DBModels;
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
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkforce([FromBody] WorkforceDTO.Request workforceRequest) {
            try
            {
                return Ok(await WorkforcesService.CreateWorkforce(workforceRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteWorkforce(string id) {
            try
            {
                var workforceDTORequest = WorkforcesService.GetWorkforceById(id);
                if(workforceDTORequest == null) {
                    return BadRequest("Cannot found this workforce!");
                }
                await WorkforcesService.DeleteWorkforce(await workforceDTORequest);
                return Ok("Delete success");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPut("id")]
        public async Task<IActionResult> UpdateWorkforce(string id, WorkforceDTO.Request workforceRequest) {
            try
            {
                var w1 = WorkforcesService.GetWorkforceById(id);
                if(w1 == null) {
                    return BadRequest("Cannot found this workforce!");
                }
                await WorkforcesService.UpdateWorkforce(workforceRequest);
                return Ok("Update success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
