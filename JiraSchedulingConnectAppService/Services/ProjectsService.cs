using AutoMapper;
using Elastic.CommonSchema;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs;
using ModelLibrary.DTOs.Projects;
using UtilsLibrary;
using UtilsLibrary.Exceptions;

namespace JiraSchedulingConnectAppService.Services
{
    public class ProjectsService : IProjectServices
    {
        private readonly WoTaasContext db;
        private readonly IMapper mapper;
        private readonly HttpContext? httpContext;
        private readonly IAuthorizationService _authorizationService;
        public ProjectsService(WoTaasContext dbContext, IMapper mapper, IAuthorizationService _authorizationService, IHttpContextAccessor httpContextAccessor)
        {
            db = dbContext;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext;
            this._authorizationService = _authorizationService;

        }

        async public Task<List<ProjectListHomePageDTO>> GetAllProjects(string? projectName)
        {
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();
            projectName = projectName ?? string.Empty;

            //QUERY LIST PROJECT WITH CLOUD_ID
            var query = db.Projects.Where(e => e.CloudId == cloudId
                && (projectName.Equals(string.Empty) || e.Name.Contains(projectName)
                && e.IsDelete == false)
                ).Include(p => p.Tasks)
                .OrderByDescending(e => e.Id);
            var projectsResult = await query.ToListAsync();
            var projectDTO = mapper.Map<List<ProjectListHomePageDTO>>(projectsResult);

            return projectDTO;
        }

        async public Task<PagingResponseDTO<ProjectListHomePageDTO>>
            GetAllProjectsPaging(int currentPage, string? projectName)
        {
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            projectName = projectName ?? string.Empty;

            var query = db.Projects.Where(e => e.CloudId == cloudId
                && (projectName.Equals(string.Empty) || e.Name.Contains(projectName))
                && e.IsDelete == true
                ).Include(p => p.Tasks)
                .OrderByDescending(e => e.Id);

            var queryPagingResult = Utils.MyQuery<ModelLibrary.DBModels.Project>.Paging(query, currentPage);
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

        public async Task<ProjectDetailDTO> CreateProject(ProjectsListCreateProject projectRequest)
        {
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            var planId = await db.Subscriptions.Include(s => s.AtlassianToken)
                .Include(s => s.Plan)
                .Where(s => s.AtlassianToken.CloudId == cloudId && s.CancelAt == null)
                .Select(S => S.PlanId).FirstOrDefaultAsync();
            int ActivateProjectUsage = await GetActiveProjectUsage();

            // Validate Permission
            await _authorizationService.AuthorizeAsync(httpContext.User, new ModelLibrary.DTOs.Algorithm.UserUsage()
            {
                Plan = (int)planId,
                ProjectActiveUsage = ActivateProjectUsage

            }, "LimitedCreateProject");

            projectRequest = ValidateProjectInput(projectRequest);

            // validate project name exited
            await _IsValidateExitedName(projectRequest.Name);


            var project = mapper.Map<ModelLibrary.DBModels.Project>(projectRequest);



            project.CloudId = cloudId;


            // Check Name project's exited
            // if not exited -> insert
            // else throw error
            var existingProject = await db.Projects
                .FirstOrDefaultAsync(p => p.Name == project.Name && p.CloudId == cloudId);

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

        public async Task<ProjectDetailDTO> UpdateProject(int projectId, ProjectsListCreateProject projectRequest)
        {

            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            projectRequest = ValidateProjectInput(projectRequest);

            var projectUpdate = mapper.Map<ModelLibrary.DBModels.Project>(projectRequest);
            projectUpdate.CloudId = cloudId;

            var projectInDB = await db.Projects.FirstOrDefaultAsync(p => p.Id == projectId) ??
                throw new NotFoundException($"Can not find project :{projectId}");

            // Check Name project's exited
            // if not exited -> insert
            // else throw error
            var existingProject = await db.Projects
                .FirstOrDefaultAsync(p => p.Name == projectUpdate.Name &&
                p.CloudId == cloudId && p.Name != projectInDB.Name);
            if (existingProject != null)
            {
                // Or handle the situation accordingly
                throw new DuplicateException(Const.MESSAGE.PROJECT_NAME_EXIST);
            }

            projectInDB.Name = projectUpdate.Name;
            projectInDB.Deadline = projectUpdate.Deadline;
            projectInDB.Budget = projectUpdate.Budget;
            projectInDB.BudgetUnit = projectUpdate.BudgetUnit;
            projectInDB.StartDate = projectUpdate.StartDate;
            projectInDB.ImageAvatar = projectUpdate.ImageAvatar;
            projectInDB.BaseWorkingHour = projectUpdate.BaseWorkingHour;
            projectInDB.ObjectiveCost = projectUpdate.ObjectiveCost;
            projectInDB.ObjectiveQuality = projectUpdate.ObjectiveQuality;
            projectInDB.ObjectiveTime = projectUpdate.ObjectiveTime;
            projectInDB.WorkingTimes = projectUpdate.WorkingTimes;

            var projectUpdatedEntity = db.Projects.Update(projectInDB);
            await db.SaveChangesAsync();
            var projectUpdatedDTO = mapper.Map<ProjectDetailDTO>(projectUpdatedEntity.Entity);
            return projectUpdatedDTO;
        }

        private async Task<bool> _IsValidateExitedName(string projectName)
        {
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            var existingProject = await db.Projects
               .FirstOrDefaultAsync(
                p => p.Name == projectName && p.CloudId == cloudId && p.IsDelete == false);

            if(existingProject != null)
            {
                throw new DuplicateException(Const.MESSAGE.PROJECT_NAME_EXIST);
            }
            

            return true;
        }


        private ProjectsListCreateProject ValidateProjectInput(ProjectsListCreateProject projectRequest)
        {

            if(projectRequest.Name == null)
            {
                throw new Exception(Const.MESSAGE.PROJECT_NAME_IS_NULL);
            }
            // validate project name
            projectRequest.Name = projectRequest.Name.Trim();

            if (projectRequest.Name == string.Empty)
            {
                throw new Exception(Const.MESSAGE.PROJECT_NAME_EMPTY);
            }


            if (!Utils.IsUpperFirstLetter(projectRequest.Name))
                throw new Exception(Const.MESSAGE.PROJECT_NAME_UPPER_1ST_CHAR);


            // validate Unit

            if (projectRequest.BudgetUnit == null)
            {
                throw new Exception(Const.MESSAGE.UNIT_EMPTY);
            }

            projectRequest.BudgetUnit = projectRequest.BudgetUnit.Trim();


            if ( projectRequest.BudgetUnit.Trim() == "")
            {
                throw new Exception(Const.MESSAGE.UNIT_EMPTY);
            }

            


            // validate start date

            if (projectRequest.StartDate == null)
            {
                throw new Exception(Const.MESSAGE.START_DATE_INVALIDATE);
            }

            if (projectRequest.StartDate == null)
            {
                throw new Exception(Const.MESSAGE.END_DATE_INVALIDATE);
            }


            if (projectRequest.Budget < 0)
            {
                throw new Exception(Const.MESSAGE.PROJECT_BUDGET_ERR);
            }
            projectRequest.Deadline = Utils.MoveDayToEnd(projectRequest.Deadline);

            var baseWorkingHour = 0d;
            for (int i = 0; i < projectRequest.WorkingTimes.Count(); i++)
            {
                var wTime = projectRequest.WorkingTimes[i];
                var start = TimeOnly.Parse(wTime.Start);
                var finish = TimeOnly.Parse(wTime.Finish);
                if (i > 0 && start < TimeOnly.Parse(projectRequest.WorkingTimes[i - 1].Finish))
                {
                    throw new Exception(Const.MESSAGE.WORKING_TIME_INVALID);

                }
                if (start >= finish)
                    throw new Exception(Const.MESSAGE.WORKING_TIME_INVALID);

                baseWorkingHour += (finish - start).TotalMinutes / 60;
            }
            projectRequest.BaseWorkingHour = baseWorkingHour;

            return projectRequest;
        }

        public async Task<ProjectDeleteResDTO> DeleteProject(int projectId)
        {

            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            var projectInDB = await db.Projects.FirstOrDefaultAsync(p => p.Id == projectId
                && p.CloudId == cloudId) ??
                throw new NotFoundException($"Can not find project :{projectId}");

            projectInDB.IsDelete = Const.DELETE_STATE.DELETE;
            projectInDB.DeleteDatetime = DateTime.Now;

            var projectUpdatedEntity = db.Projects.Update(projectInDB);
            await db.SaveChangesAsync();
            var projectUpdatedDTO = mapper.Map<ProjectDeleteResDTO>(projectUpdatedEntity.Entity);
            return projectUpdatedDTO;
        }

        public async Task<int> GetActiveProjectUsage()
        {
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            var CreatedProjectnumber = await db.Projects.Where(p => p.CloudId == cloudId && p.IsDelete == false).CountAsync();
            return CreatedProjectnumber;
        }
    }
}

