using ModelLibrary.DTOs.Skills;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface ISkillsService
    {
        public Task<SkillDTOResponse> GetSkillName(string? skillName);

        public Task<List<SkillDTOResponse>> GetSkills(string? skillName);


        public Task<SkillDTOResponse> GetSkillId(int Id);



        public Task<SkillDTOResponse> UpdateNameSkill(SkillDTOResponse skill);

        public Task<bool> DeleteSkill(int Id);

        public Task<SkillDTOResponse> CreateSkill(SkillCreatedRequest skillRequest);

    }
}
