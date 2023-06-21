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
        public async Task<IActionResult> GetAllProjects(int page)
        {
            try
            {
                var response = await projectsService.GetAllProject(page);
                return Ok(response);
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
                await projectsService.CreateProject(projectRequest);
                return Ok();
            }
            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }
    }
}
