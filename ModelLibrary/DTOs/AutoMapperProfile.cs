using AutoMapper;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Projects;

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
        }
    }
}
