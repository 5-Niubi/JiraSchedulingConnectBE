﻿using AutoMapper;
using JiraSchedulingConnectAppService.Common;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs;
using ModelLibrary.DTOs.Projects;
using UtilsLibrary.Exceptions;

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

        public async System.Threading.Tasks.Task<ProjectDetailDTO> CreateProject(ProjectsListCreateProject projectRequest)
        {
            try
            {
                var jwt = new JWTManagerService(httpContext);
                var cloudId = jwt.GetCurrentCloudId();

                var project = mapper.Map<Project>(projectRequest);
                project.CloudId = cloudId;

                // Check Name project's exited
                // if not exited -> insert
                // else throw error
                var existingProject = await db.Projects.FirstOrDefaultAsync(p => p.Name == project.Name && p.CloudId == cloudId);

                if (existingProject != null)
                {
                    // Or handle the situation accordingly
                    throw new DuplicateException(Const.MESSAGE.PROJECT_NAME_EXIST);
                }

                var projectCreatedEntity = await db.Projects.AddAsync(project);
                await db.SaveChangesAsync();
                var projectCreatedDTO = mapper.Map<ProjectDetailDTO>(projectCreatedEntity.Entity);
                return projectCreatedDTO;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        async public Task<List<ProjectListHomePageDTO>> GetAllProjects(string? projectName)
        {
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            projectName = projectName ?? string.Empty;

            //QUERY LIST PROJECT WITH CLOUD_ID
            //var query = db.Projects.Where(e => e.CloudId == cloudId
            //    && (projectName.Equals(string.Empty) || e.Name.Contains(projectName))
            //    )
            //    .OrderByDescending(e => e.Id);

            ///////////////////////////////////////
            //QUERY LIST PROJECT WITHOUT CLOUD_ID (TEMPORARY FIX)
            var query = db.Projects.Where(e => (projectName.Equals(string.Empty) || e.Name.Contains(projectName)))
                .OrderByDescending(e => e.Id);
            ///////////////////////////////////////
            

            var projectsResult = await query.ToListAsync();
            var projectDTO = mapper.Map<List<ProjectListHomePageDTO>>(projectsResult);

            return projectDTO;
        }

        async public Task<PagingResponseDTO<ProjectListHomePageDTO>> GetAllProjectsPaging(int currentPage, string? projectName)
        {
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            projectName = projectName ?? string.Empty;

            var query = db.Projects.Where(e => e.CloudId == cloudId
                && (projectName.Equals(string.Empty) || e.Name.Contains(projectName))
                )
                .OrderByDescending(e => e.Id);

            var queryPagingResult = Utils.MyQuery<Project>.Paging(query, currentPage);
            var projectsResult = await queryPagingResult.Item1.ToListAsync();
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

        async public Task<ProjectDetailDTO> GetProjectDetail(int projectId)
        {
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();
            var projectResult = await db.Projects.Where(e => e.CloudId == cloudId && e.Id == projectId)
                .FirstOrDefaultAsync();
            var projectDTO = mapper.Map<ProjectDetailDTO>(projectResult);
            return projectDTO;
        }
    }
}
