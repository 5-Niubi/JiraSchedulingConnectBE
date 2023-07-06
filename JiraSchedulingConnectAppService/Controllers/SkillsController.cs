using JiraSchedulingConnectAppService.Services;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;
using ModelLibrary.DTOs.Projects;
using ModelLibrary.DTOs.Skills;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SkillsController : ControllerBase
    {
        private readonly ISkillsService SkillsService;
        public SkillsController(ISkillsService skillsService)
        {
            this.SkillsService = skillsService;
        }

        [HttpGet]
        async public Task<IActionResult> GetSkills(string? Name)
        {
            try
            {
                var response = await SkillsService.GetSkills(Name);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSkill([FromBody] SkillsListCreateSkill.Request skillRequest)
        {
            try
            {
                return Ok(await SkillsService.CreateSkill(skillRequest));
            }

            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }


        [HttpPut("{id}")]
        async public Task<IActionResult> UpdateNameSkill(int id, [FromBody] SkillDTO skill)
        {
            try
            {
                // validate input
                if(skill.Name == string.Empty) {
                    return BadRequest("Name not empty");
                }

                // validate exited & unique skill name
                var exitedSkill = await SkillsService.GetSkillId(id);
                var exitedName = await SkillsService.GetSkillName(skill.Name);

                if (exitedSkill == null)
                {
                    return BadRequest("Cannot found this skill!");
                }

                else if (exitedName != null)
                {
                    return BadRequest("Skill Name must unique!");
                }


                // update skill name
                var response = await SkillsService.UpdateNameSkill(id, skill);

                return Ok("Update success");

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
