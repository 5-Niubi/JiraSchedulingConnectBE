using AutoMapper;
using JiraSchedulingConnectAppService.DTOs.Projects;
using ModelLibrary.DBModels;


namespace JiraSchedulingConnectAppService.DTOs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Project, ProjectListHomePageDTO>();
            CreateMap<ProjectsListCreateProject.Request, Project>();
            CreateMap<Project, ProjectDetailDTO>();

        }
    }
}
