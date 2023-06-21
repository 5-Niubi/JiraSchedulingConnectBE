using AutoMapper;
using JiraSchedulingConnectAppService.DTOs.Projects;
using JiraSchedulingConnectAppService.Models;

namespace JiraSchedulingConnectAppService.DTOs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Project, ProjectListHomePageDTO>();
            CreateMap<ProjectsListCreateProject.Request, Project>();
            
        }
    }
}
