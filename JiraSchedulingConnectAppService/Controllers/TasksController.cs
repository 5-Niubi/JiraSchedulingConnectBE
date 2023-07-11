
﻿using JiraSchedulingConnectAppService.Services;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;
using ModelLibrary.DTOs.PertSchedule;
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
        public TasksController(ITasksService tasksService)
            
        {
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

            catch (Exception ex)
            {
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

            catch (Exception ex)
            {
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
            catch (Exception ex)
            {
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
            catch (Exception ex)
            {
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
            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }


        [HttpPost]
        public async Task<IActionResult> SaveTasksPrecedencesTasks(TasksPrecedencesSaveRequest taskRequest)
        {

            try
            {
                var resopnse = await TasksService.SaveTasksPrecedencesTasks(taskRequest);
                return Ok(resopnse);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }



        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return NoContent();
        }





        

    }
}

