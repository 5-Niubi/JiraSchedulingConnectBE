using JiraSchedulingConnectAppService.DTOs.Projects;
using JiraSchedulingConnectAppService.DTOs;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IProjectServices
    {
        public PagingResponseDTO<ProjectListHomePageDTO> GetAllProject(HttpContext context, int currentPage);
    }
}
