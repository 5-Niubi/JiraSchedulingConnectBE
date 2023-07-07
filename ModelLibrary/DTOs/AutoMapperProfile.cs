using AutoMapper;
using ModelLibrary.DBModels;
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
            CreateMap<WorkforceDTO.Request, Workforce>();
            CreateMap<Workforce, WorkforceDTO.Response>();
            CreateMap<EquipmentDTO.Request, Equipment>();
            CreateMap<Equipment, EquipmentDTO.Response>();
            CreateMap<Skill, SkillDTO>();
            CreateMap<SkillDTO, Skill>();
            CreateMap<SkillsListCreateSkill.Request, Skill>();
        }
    }
}
