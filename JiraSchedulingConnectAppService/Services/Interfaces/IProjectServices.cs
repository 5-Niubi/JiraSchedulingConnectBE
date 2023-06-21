using JiraSchedulingConnectAppService.DTOs.Projects;
using JiraSchedulingConnectAppService.DTOs;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IProjectServices
    {
         public Task< PagingResponseDTO<ProjectListHomePageDTO>> GetAllProject( int currentPage);
         public Task CreateProject(ProjectsListCreateProject.Request projectRequest);
    }
}
