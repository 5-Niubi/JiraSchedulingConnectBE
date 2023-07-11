using JiraSchedulingConnectAppService.Common;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;
using ModelLibrary.DTOs.Skills;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SkillsController : ControllerBase
    {

        private readonly ISkillsService skillsService;
        public SkillsController(ISkillsService skillsService)
        {
            this.skillsService = skillsService;
        }

        [HttpGet]
        async public Task<IActionResult> GetSkills(string? name)
        {
            try
            {
                var response = await skillsService.GetSkills(name);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSkill([FromBody] SkillCreatedRequest skillRequest)
        {
            try
            {
                var response = await skillsService.CreateSkill(skillRequest);
                return Ok(response);
            }

            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }


        [HttpPut]
        async public Task<IActionResult> UpdateNameSkill( [FromBody] SkillDTO skill)
        {
            try
            {

                // update skill name
                var result = await skillsService.UpdateNameSkill(skill);
                var response = new ResponseMessageDTO(Const.MESSAGE.SUCCESS);
                response.Data = result;

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpDelete]
        async public Task<IActionResult> DeleteSkill(int id)
        {
            try
            {
                await skillsService.DeleteSkill(id);
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
