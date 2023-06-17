using AutoMapper;
using JiraSchedulingConnectAppService.DTOs;
using JiraSchedulingConnectAppService.Models;
using JiraSchedulingConnectAppService.Services;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JiraSchedulingConnectAppService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private IProjectServices projectsService;
        public ProjectsController(IProjectServices projectsService) {
            this.projectsService = projectsService;
        }

        [HttpGet]
        public IActionResult Index(int page)
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
