using ModelLibrary.DTOs;
using ModelLibrary.DTOs.Projects;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IProjectServices
    {
        public Task<PagingResponseDTO<ProjectListHomePageDTO>> GetAllProjectsPaging(int currentPage, string? projectName);
        public Task<List<ProjectListHomePageDTO>> GetAllProjects(string? projectName);
        public Task<ProjectDetailDTO> GetProjectDetail(int projectId);

        public Task<ProjectDetailDTO> CreateProject(ProjectsListCreateProject projectRequest);
        public Task<ProjectDetailDTO> UpdateProject(int projectId, ProjectsListCreateProject projectRequest);
        public  Task<ProjectDeleteResDTO> DeleteProject(int projectId);
    }
}
