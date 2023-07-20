using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary;
using ModelLibrary.DTOs;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService scheduleService;
        private readonly ILoggerManager _Logger;
        public ScheduleController(IScheduleService scheduleService, ILoggerManager logger)
        {

            this._Logger = logger;
            this.scheduleService = scheduleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSchedulesByProject(int projectId)
        {
            try
            {
                var response = await scheduleService.GetSchedulesByProject(projectId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                this._Logger.LogDebug(ex.Message);

                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSchedules(int parameterId)
        {
            try
            {
                var response = await scheduleService.GetSchedules(parameterId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                this._Logger.LogDebug(ex.Message);

                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSchedule(int scheduleId)
        {
            try
            {
                var response = await scheduleService.GetSchedule(scheduleId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                this._Logger.LogDebug(ex.Message);

                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

    }
}
