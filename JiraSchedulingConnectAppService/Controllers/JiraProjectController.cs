using JiraSchedulingConnectAppService.DTOs;
using JiraSchedulingConnectAppService.Models;
using JiraSchedulingConnectAppService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("[controller]/[action]")]
    [Authorize]
    public class JiraProjectController : ControllerBase
    {
        private readonly JiraProjectService service;
        public JiraProjectController(JiraDemoContext db, IConfiguration config)
        {
            this.service = new JiraProjectService(db, config);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllProject()
        {
            try
            {
                var allProject = await service.GetAllProject(HttpContext);
                return Ok(allProject);
            }
            catch (Exception ex)
            {
                var responseMsg = new ResponseMessageDTO<Object>(ex.Message);
                return BadRequest(responseMsg);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDTO.Request project)
        {
            try
            {
                return Ok(project);
            }
            catch (Exception ex)
            {
                var responseMsg = new ResponseMessageDTO<Object>(ex.Message);
                return BadRequest(responseMsg);
            }
        }
    }
}
