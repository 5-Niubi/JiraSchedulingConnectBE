using ModelLibrary.DBModels;
using JiraSchedulingConnectAppService.Services.Interfaces;

namespace JiraSchedulingConnectAppService.Services
{
    public class SkillsService : ISkillsService
    {
        private readonly JiraDemoContext db;
        public SkillsService(JiraDemoContext db) { 
            
            this.db = db;
        }

        public List<Skill> GetSkills()
        {
            var skills = db.Skills.ToList();
            return skills;
        }
    }
}
