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
        private readonly JiraDemoContext db;
        private readonly IMapper mapper;
        private readonly HttpContext? httpContext;

        public ProjectsService(JiraDemoContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            db = dbContext;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext;

        }

        public async System.Threading.Tasks.Task CreateProject(ProjectsListCreateProject.Request projectRequest)
        {
            try
            {
                var jwt = new JWTManagerService(httpContext);
                var cloudId = jwt.GetCurrentCloudId();

                var project = mapper.Map<Project>(projectRequest);
                project.CloudId = cloudId;
                await db.Projects.AddAsync(project);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        async public Task<PagingResponseDTO<ProjectListHomePageDTO>> GetAllProject(int currentPage)
        {
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

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
