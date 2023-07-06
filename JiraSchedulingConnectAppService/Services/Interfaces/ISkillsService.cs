﻿using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Skills;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface ISkillsService
    {
        public Task<SkillDTO> GetSkillName(string? skillName);

        public Task<List<SkillDTO>> GetSkills(string? skillName);
        
        public Task<Skill> GetSkillId(int Id);

        public Task<SkillDTO> UpdateNameSkill(int Id, SkillDTO skill);
        //public Task<SkillDTO> DeleteSkill();

        public Task<SkillDTO> CreateSkill(SkillsListCreateSkill.Request skillRequest);

    }
}
