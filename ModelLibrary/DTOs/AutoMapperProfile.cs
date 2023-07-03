using AutoMapper;
using ModelLibrary.DTOs.Projects;
using ModelLibrary.DBModels;

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
        }
    }
}
