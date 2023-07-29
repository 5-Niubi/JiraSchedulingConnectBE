using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;
using ModelLibrary.DTOs.PertSchedule;
using UtilsLibrary.Exceptions;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase

    {
        private readonly ITasksService TasksService;
        private readonly ModelLibrary.ILoggerManager _Logger;
        public TasksController(ITasksService tasksService, ModelLibrary.ILoggerManager logger){
            this._Logger = logger;
            this.TasksService = tasksService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskCreatedRequest taskRequest)
        {
            try
            {
                var response = await TasksService.CreateTask(taskRequest);
                return Ok(response);
            }

            catch (NotSuitableInputException ex)
            {
                this._Logger.LogWarning(ex.Message);
                var response = ex.Errors;
                return BadRequest(response);
            }

            catch (Exception ex)
            {

                this._Logger.LogError(ex.Message);
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }


        [HttpPost]
        public async Task<IActionResult> SaveTasksForPert([FromBody] TaskCreatedRequest taskRequest)
        {
            try
            {
                var response = await TasksService.CreateTask(taskRequest);
                return Ok(response);
            }

            catch (NotSuitableInputException ex)
            {
                this._Logger.LogWarning(ex.Message);
                var response = ex.Errors;
                return BadRequest(response);
            }

            catch (Exception ex)
            {

                this._Logger.LogError(ex.Message);
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }




        [HttpGet]
        public async Task<IActionResult> GetTaskDetail(int Id)
        {

            try
            {
                var response = await TasksService.GetTaskDetail(Id);
                return Ok(response);
            }

            catch (NotSuitableInputException ex)
            {
                this._Logger.LogWarning(ex.Message);
                var response = ex.Errors;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                this._Logger.LogError(ex.Message);
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTasksPertChart(int ProjectId)
        {

            try
            {
                var response = await TasksService.GetTasksPertChart(ProjectId);
                return Ok(response);
            }

            catch (NotSuitableInputException ex)
            {
                this._Logger.LogWarning(ex.Message);
                var response = ex.Errors;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                this._Logger.LogError(ex.Message);
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }


        [HttpPut]
        public async Task<IActionResult> UpdateTask(TaskUpdatedRequest taskRequest)
        {

            try
            {
                var resopnse = await TasksService.UpdateTask(taskRequest);
                return Ok(resopnse);
            }

            catch (NotSuitableInputException ex)
            {
                this._Logger.LogWarning(ex.Message);
                var response = ex.Errors;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                this._Logger.LogError(ex.Message);
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }



        

        [HttpDelete]
        public async Task<IActionResult> DeleteTask(int Id)
        {

            try
            {
                var resopnse = await TasksService.DeleteTask(Id);
                return Ok(resopnse);
            }

            catch (NotSuitableInputException ex)
            {
                this._Logger.LogWarning(ex.Message);
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                this._Logger.LogError(ex.Message);
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }


        [HttpPost]
        public async Task<IActionResult> SaveTasks(TasksSaveRequest taskRequest)
        {

            try
            {
                var resopnse = await TasksService.SaveTasks(taskRequest);
                return Ok(resopnse);
            }

            catch (NotSuitableInputException ex)
            {
                this._Logger.LogWarning(ex.Message);
                var response = ex.Errors;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                this._Logger.LogError(ex.Message);
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        // TODO SAVE TASK REQUESTS FULL INFO
        //[HttpPost]
        //public async Task<IActionResult> SaveTasksV2(TasksSaveRequestV2 taskRequest)
        //{

        //    try
        //    {
        //        var resopnse = await TasksService.TasksSaveRequestV2(taskRequest);
        //        return Ok(resopnse);
        //    }

        //    catch (NotSuitableInputException ex)
        //    {
        //        this._Logger.LogWarning(ex.Message);
        //        var response = ex.Errors;
        //        return BadRequest(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        this._Logger.LogError(ex.Message);
        //        var response = new ResponseMessageDTO(ex.Message);
        //        return BadRequest(response);
        //    }
        //}



        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return NoContent();
        }







    }
}

