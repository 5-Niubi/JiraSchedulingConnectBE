using JiraSchedulingConnectAppService.DTOs.Projects;
using JiraSchedulingConnectAppService.DTOs;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IProjectServices
    {
        public Task<PagingResponseDTO<ProjectListHomePageDTO>> GetAllProject(int currentPage);
        public Task<ProjectDetailDTO> CreateProject(ProjectsListCreateProject.Request projectRequest);
        public Task<ProjectDetailDTO> GetProjectDetail(int projectId);
    }
}
