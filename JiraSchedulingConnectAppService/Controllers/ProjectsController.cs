using AutoMapper;
using JiraSchedulingConnectAppService.DTOs;
using JiraSchedulingConnectAppService.Models;
using JiraSchedulingConnectAppService.Services;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private IProjectServices projectsService;
        public ProjectsController(IProjectServices projectsService) {
            this.projectsService = projectsService;
        }

        [HttpGet]
        public IActionResult GetAllProjects(int page)
        {
            try
            {
                var response =  projectsService.GetAllProject(HttpContext, page);
                return Ok(response);
            }catch(Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }
    }
}
