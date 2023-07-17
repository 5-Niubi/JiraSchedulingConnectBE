using JiraSchedulingConnectAppService.Common;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs.Milestones;
using ModelLibrary.DTOs;
using ModelLibrary;

namespace JiraSchedulingConnectAppService.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class MilestonesController : Controller
	{
		private readonly IMilestonesService milestonesService;
        private readonly ILoggerManager _Logger;

        public MilestonesController(IMilestonesService milestonesService, ILoggerManager logger)
		{
			this._Logger = logger;

            this.milestonesService = milestonesService;
		}

		[HttpGet]
		async public Task<IActionResult> GetMilestones(int projectId )
		{
			try
			{

				_Logger.LogWarning("ahihi");
                this._Logger.LogError("HELLO WORLD");
                var response = await milestonesService.GetMilestones(projectId);
				return Ok(response);
			}
			catch (Exception ex)
			{
                this._Logger.LogError(ex.Message);
                var response = new ResponseMessageDTO(ex.Message);
				return BadRequest(response);
			}
		}

		[HttpPost]
		public async Task<IActionResult> CreateMileStone([FromBody] MilestoneCreatedRequest milestoneRequest)
		{
			try
			{
				var response = await milestonesService.CreateMilestone(milestoneRequest);
				return Ok(response);
			}

			catch (Exception ex)
			{
                this._Logger.LogError(ex.Message);
                var response = new ResponseMessageDTO(ex.Message);
				return BadRequest(response);
			}
		}


		[HttpDelete]
		async public Task<IActionResult> DeleteMilestone(int id)
		{
			try
			{
				await milestonesService.DeleteMilestone(id);
				return Ok();
			}
			catch (Exception ex)
			{
                this._Logger.LogError(ex.Message);
                var response = new ResponseMessageDTO(ex.Message);
				return BadRequest(response);
			}
		}
	}
}
