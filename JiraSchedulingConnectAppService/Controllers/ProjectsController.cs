using JiraSchedulingConnectAppService.DTOs;
using JiraSchedulingConnectAppService.DTOs.Projects;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private IProjectServices projectsService;
        public ProjectsController(IProjectServices projectsService)
        {
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
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectsListCreateProject.Request projectRequest)
        {
            try
            {
                return Ok(await projectsService.CreateProject(projectRequest));
            }
            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }
    }
}
