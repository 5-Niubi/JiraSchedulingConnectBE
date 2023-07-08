using AutoMapper;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Algorithm;
using ModelLibrary.DTOs.Projects;
using ModelLibrary.DTOs.Skills;

namespace ModelLibrary.DTOs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Project, ProjectListHomePageDTO>();
            CreateMap<ProjectsListCreateProject.Request, Project>();
            CreateMap<Project, ProjectDetailDTO>();
            CreateMap<WorkforceDTO, Workforce>();
            CreateMap<Skill, SkillDTO>();
            CreateMap<SkillDTO, Skill>();
            CreateMap<SkillsListCreateSkill.Request, Skill>();
            CreateMap<Schedule, ScheduleResultSolutionDTO>();
        }
    }
}
