using JiraSchedulingConnectAppService.Services;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;
using ModelLibrary.DTOs.Skills;
using ModelLibrary.DTOs.Tasks;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TasksController: ControllerBase
    {
        private readonly ITasksService TasksService;
        public TasksController(TasksService tasksService)
            
        {
            TasksService = tasksService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TasksListCreateTask.Request taskRequest)
        {
            try
            {
                var response = await TasksService.CreateTask(taskRequest);
                return Ok(response);
            }

            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

    }
}

