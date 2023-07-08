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
                var response = await SkillsService.CreateSkill(skillRequest);
                return Ok(response);
            }

            catch (Exception ex)
            {
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }


        [HttpPut]
        async public Task<IActionResult> UpdateNameSkill(int id, [FromBody] SkillDTO skill)
        {
            try
            {
                // update skill name
                var result = await SkillsService.UpdateNameSkill(id, skill);
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
                await SkillsService.DeleteSkill(id);
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
