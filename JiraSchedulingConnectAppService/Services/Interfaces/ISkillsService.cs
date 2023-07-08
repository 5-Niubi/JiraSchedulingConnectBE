using ModelLibrary.DTOs.Skills;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface ISkillsService
    {
        public Task<SkillDTO> GetSkillName(string? skillName);

        public Task<List<SkillDTO>> GetSkills(string? skillName);


        public Task<SkillDTO> GetSkillId(int Id);



        public Task<SkillDTO> UpdateNameSkill(int Id, SkillDTO skill);

        public Task<bool> DeleteSkill(int Id);

        public Task<SkillDTO> CreateSkill(SkillsListCreateSkill.Request skillRequest);

    }
}
