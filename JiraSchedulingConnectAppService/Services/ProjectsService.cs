using AutoMapper;
using JiraSchedulingConnectAppService.Common;
using JiraSchedulingConnectAppService.DTOs;
using JiraSchedulingConnectAppService.DTOs.Projects;
using JiraSchedulingConnectAppService.Models;
using JiraSchedulingConnectAppService.Services.Interfaces;

namespace JiraSchedulingConnectAppService.Services
{
    public class ProjectsService : IProjectServices
    {
        private JiraDemoContext db;
        private IMapper mapper;
        public ProjectsService(JiraDemoContext dbContext, IMapper mapper)
        {
            db = dbContext;
            this.mapper = mapper;
        }

        public PagingResponseDTO<ProjectListHomePageDTO> GetAllProject(HttpContext context, int currentPage)
        {
            var cloudId = JWTManagerService.GetCurrentCloudId(context);

            var query = db.Projects.Where(e => e.CloudId == cloudId);

            var queryPagingResult = Utils.MyQuery<Project>.Paging(query, currentPage);
            var projectsResult = queryPagingResult.Item1.ToList();


            var projectDTO = mapper.Map<List<ProjectListHomePageDTO>>(projectsResult);

            var pagingRespone = new PagingResponseDTO<ProjectListHomePageDTO>
            {
                MaxResults = queryPagingResult.Item4,
                PageIndex = queryPagingResult.Item3,
                Total = queryPagingResult.Item2,
                Values = projectDTO
            };
            return pagingRespone;
        }
    }
}
