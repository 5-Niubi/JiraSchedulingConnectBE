using AutoMapper;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Algorithm;
using ModelLibrary.DTOs.Algorithm.ScheduleResult;
using ModelLibrary.DTOs.Parameters;
using ModelLibrary.DTOs.Projects;
using ModelLibrary.DTOs.Skills;

namespace ModelLibrary.DTOs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Project, ProjectListHomePageDTO>();
            CreateMap<ProjectsListCreateProject, Project>();
            CreateMap<Project, ProjectDetailDTO>();
            CreateMap<WorkforceDTORequest, Workforce>();
            CreateMap<Workforce, WorkforceDTORequest>();
            CreateMap<Workforce, WorkforceDTOResponse>();
            CreateMap<WorkforceDTOResponse, Workforce>();
            CreateMap<EquipmentDTOResponse, Equipment>();
            CreateMap<Equipment, EquipmentDTOResponse>();
            CreateMap<Skill, SkillDTO>();
            CreateMap<SkillDTO, Skill>();
            CreateMap<SkillsListCreateSkill.Request, Skill>();
            CreateMap<Schedule, ScheduleResultSolutionDTO>();
            CreateMap<Workforce, WorkforceScheduleResultDTO>();
        }
    }
}
