using ModelLibrary.DTOs;
using ModelLibrary.DTOs.Projects;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IProjectServices
    {
        public Task<PagingResponseDTO<ProjectListHomePageDTO>> GetAllProjectsPaging(int currentPage, string? projectName);
        public Task<List<ProjectListHomePageDTO>> GetAllProjects(string? projectName);
        public Task<ProjectDetailDTO> CreateProject(ProjectsListCreateProject.Request projectRequest);
        public Task<ProjectDetailDTO> GetProjectDetail(int projectId);
    }
}
