using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;
using ModelLibrary.DTOs.Parameters;
using UtilsLibrary.Exceptions;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize]
    public class WorkforcesController : ControllerBase
    {
        private IWorkforcesService workforcesService;
        private readonly ILoggerService _Logger;
        public WorkforcesController(IWorkforcesService workforcesService, ILoggerService logger)
        {
            this._Logger = logger;

            this.workforcesService = workforcesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWorkforces()
        {
            try
            {
                var response = await workforcesService.GetAllWorkforces(null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                this._Logger.Log(LogLevel.Error, ex);
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkforce([FromBody] WorkforceRequestDTO workforce)
        {
            try
            {
                return Ok(await workforcesService.CreateWorkforce(workforce));
            }

            catch (NotSuitableInputException ex)
            {
                this._Logger.Log(LogLevel.Warning, ex);
                var response = ex.Errors;
                return BadRequest(response);

         
            }

            
            catch (Exception ex)
            {
                this._Logger.Log(LogLevel.Error, ex);
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
                this._Logger.Log(LogLevel.Error, ex);
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteWorkforce(string id) {
            try
            {
                var w = workforcesService.GetWorkforceById(id);
                var response = await workforcesService.DeleteWorkforce(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                this._Logger.Log(LogLevel.Error, ex);
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
                var response = await workforcesService.UpdateWorkforce(workforce);
                return Ok(response);
            }

            catch (Exception ex)
            {
                this._Logger.Log(LogLevel.Error, ex);
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }
    }
}
