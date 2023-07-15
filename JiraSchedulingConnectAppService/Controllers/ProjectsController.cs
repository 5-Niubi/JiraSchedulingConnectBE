using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;
using ModelLibrary.DTOs.Projects;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly ILoggerService _Logger;
        private IProjectServices projectsService;
        public ProjectsController(IProjectServices projectsService, ILoggerService logger)
        {
            this._Logger = logger;
            this.projectsService = projectsService;
            
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjectsPaging(int page, string? projectName)
        {
            try
            {
                var response = await projectsService.GetAllProjectsPaging(page, projectName);
                return Ok(response);
            }

            catch (Exception ex)
            {
                this._Logger.Log(LogLevel.Error, ex);
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjects(string? projectName)
        {
            try
            {
                var response = await projectsService.GetAllProjects(projectName);
                return Ok(response);
            }
            catch (Exception ex)
            {
                this._Logger.Log(LogLevel.Error, ex);
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProject(int projectId)
        {
            try
            {
                var project = await projectsService.GetProjectDetail(projectId);
                return Ok(project);
            }
            catch (Exception ex)
            {
                this._Logger.Log(LogLevel.Error, ex);
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectsListCreateProject projectRequest)
        {
            try
            {
                var projectCreated = await projectsService.CreateProject(projectRequest);
                return Ok(projectCreated);
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
