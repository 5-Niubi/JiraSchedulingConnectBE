using AlgorithmLibrary;
using AutoMapper;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Algorithm;
using ModelLibrary.DTOs.Invalidation;
using ModelLibrary.DTOs.PertSchedule;
using ModelLibrary.DTOs.Thread;
using System.Dynamic;
using UtilsLibrary;
using UtilsLibrary.Exceptions;
using static UtilsLibrary.Const;

namespace JiraSchedulingConnectAppService.Services
{
    public class AlgorithmService : IAlgorithmService
    {
        private readonly IAPIMicroserviceService apiMicro;
        private readonly IThreadService threadService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IMapper mapper;

        private readonly WoTaasContext db;
        private readonly HttpContext? httpContext;

        public const string PrecedenceIsCycleMessage = "Tasks be cycle!";
        public const string SkillEmptyInTaskMessage = "Skils is Empty in this Task!";

        public AlgorithmService(
            WoTaasContext db,
            IAuthorizationService _authorizationService,
            IHttpContextAccessor httpContextAccessor,
            IAPIMicroserviceService apiMicro,
            IMapper mapper,
            IThreadService threadService)
        {
            this.apiMicro = apiMicro;
            this.threadService = threadService;
            this.db = db;
            httpContext = httpContextAccessor.HttpContext;
            this._authorizationService = _authorizationService;
            this.mapper = mapper;
        }


        public async System.Threading.Tasks.Task IsValidExecuteAuthorize()
        {

            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            var planId = await db.Subscriptions.Include(s => s.AtlassianToken)
                .Include(s => s.Plan)
                .Where(s => s.AtlassianToken.CloudId == cloudId && s.CancelAt == null)
                .Select(S => S.PlanId).FirstOrDefaultAsync();

            int scheduleDailyUsage = await _GetScheduleCurrentDayUsage();

            await _authorizationService.AuthorizeAsync(httpContext.User, new ModelLibrary.DTOs.Algorithm.UserUsage()
            {
                Plan = (int)planId,
                ScheduleUsage = scheduleDailyUsage

            }, "LimitedScheduleTimeByDay");
        }


        public async Task<int> GetScheduleMonthlyUsage()
        {

            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();
            var ProjectIds = await db.Projects.Where(pr => pr.CloudId == cloudId).Select(p => p.Id).ToArrayAsync();

            DateTime currentMonthStart = new(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime currentMonthEnd = currentMonthStart.AddMonths(1).AddTicks(-1);

            var MonthlyUsage = await db.Parameters
                .Where(pr => ProjectIds.Contains(pr.Id) && pr.CreateDatetime >= currentMonthStart && pr.CreateDatetime <= currentMonthEnd).Distinct()
                .CountAsync();
            return MonthlyUsage;

        }
   
        public async Task<LimitedAlgorithmDTO> GetExecuteAlgorithmLimited()
        {

            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            var planId = await db.Subscriptions.Include(s => s.AtlassianToken)
                .Include(s => s.Plan)
                .Where(s => s.AtlassianToken.CloudId == cloudId && s.CancelAt == null)
                .Select(S => S.PlanId).FirstOrDefaultAsync();

            var dailyUsage = await _GetScheduleCurrentDayUsage();

            var LimitedExecuteAlgorithm = 10000;

            if (planId == SUBSCRIPTION.PLAN_FREE)
            {
                LimitedExecuteAlgorithm = LIMITED_PLAN.LIMIT_DAILY_EXECUTE_ALGORITHM;
            }
            var output = new LimitedAlgorithmDTO()
            {
                planId = (int)planId,
                UsageExecuteAlgorithm = (int)   dailyUsage,
                LimitedExecuteAlgorithm = LimitedExecuteAlgorithm,
                IsAvailable = dailyUsage < LimitedExecuteAlgorithm ? 1 : 0


            };
            return output;



        }


        private async Task<int> _GetScheduleCurrentDayUsage()
        {
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            var projectIds = await db.Projects.Where(pr => pr.CloudId == cloudId).Select(p => p.Id).ToArrayAsync();

            DateTime currentDate = DateTime.Now.Date; // Get the current date (without time)

            DateTime dayStart = currentDate; // Midnight of the current day
            DateTime dayEnd = currentDate.AddDays(1).AddTicks(-1); // End of the current day

            var dailyUsage = await db.Parameters
                .Where(pr => projectIds.Contains((int)pr.ProjectId) && pr.CreateDatetime >= dayStart && pr.CreateDatetime <= dayEnd)
                .CountAsync();

            return dailyUsage;
        }



        public ThreadStartDTO ExecuteAlgorithm(int parameterId)
        {

            string threadId = ThreadService.CreateThreadId();
            threadId = threadService.StartThread(threadId,
                async () => await ProcessTestConverterThread(threadId, parameterId));

            return new ThreadStartDTO(threadId);
        }

        private async System.Threading.Tasks.Task ProcessTestConverterThread(string threadId, int parameterId)
        {
            try
            {
                var thread = threadService.GetThreadModel(threadId);
                try
                {
                    // Your thread processing logic goes here
                    var response = await apiMicro
                      .Get($"/api/Algorithm/ExecuteAlgorithm?parameterId={parameterId}");
                    dynamic responseContent;

                    responseContent = await response.Content.ReadAsStringAsync();
                    // Update the thread status and result when finished
                    thread.Status = Const.THREAD_STATUS.SUCCESS;
                    thread.Result = responseContent;
                }
                catch (MicroServiceAPIException ex)
                {
                    
                    thread.Status = Const.THREAD_STATUS.ERROR;
                    dynamic error = new ExpandoObject();
                    error.message = ex.Message;
                    error.response = ex.mircoserviceResponse;

                    thread.Result = error;
                    throw new Exception(ex.Message);
                }
                catch (NotFoundException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    thread.Status = Const.THREAD_STATUS.ERROR;

                    dynamic error = new ExpandoObject();
                    error.message = ex.Message;
                    error.stackTrace = ex.StackTrace;
                    thread.Result = error;
                    throw new Exception(ex.Message);
                }
            }
            catch {/* Do nothing*/ }
        }


        public async Task<EstimatedResultDTO> EstimateWorkforce(int projectId)
        {


            //try
            //{

            // validate project id exited
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            var projectInDB = await db.Projects.FirstOrDefaultAsync(p => p.Id == projectId && p.CloudId == cloudId) ??
            throw new NotFoundException($"Can not find project :{projectId}");


            // get list task by project
            var TaskList = await db.Tasks.Include(s => s.TaskPrecedenceTasks).Include(s => s.TasksSkillsRequireds).Where(t => t.ProjectId == projectId && t.IsDelete == false).ToArrayAsync();

            // validate all task must have skills
            await _ValidateExitedSkillInTask(TaskList);

            // validate graph tasks is cycle
            await _ValidateDAG(TaskList);

            var response = await apiMicro.Get($"/api/WorkforceEstimator/GetEstimateWorkforce?projectId={projectId}");
            dynamic responseContent;

            if (response.IsSuccessStatusCode)
            {
                responseContent = await response.Content.ReadFromJsonAsync<EstimatedResultDTO>();
            }

            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
            return responseContent;

            //}


            //catch (MicroServiceAPIException ex)
            //{
            //    throw new Exception(ex.mircoserviceResponse);

            //}
            //catch (NotSuitableInputException ex)
            //{
            //    throw new NotSuitableInputException(ex.Errors);

            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}


        }


        private async Task<bool> _ValidateExitedSkillInTask(ModelLibrary.DBModels.Task[] TaskList)
        {

            var Errors = new List<TaskInputErrorV2DTO>();
            foreach (var task in TaskList) {
                if(task.TasksSkillsRequireds.Count == 0)
                {
                    Errors.Add(new TaskInputErrorV2DTO()
                    {
                        TaskId = task.Id,
                        Messages = SkillEmptyInTaskMessage
                    });
                }

            }

            if (Errors.Count != 0)
            {
                throw new NotSuitableInputException(Errors);

            }
            return true;

        }


        private async Task<bool> _ValidateDAG(ModelLibrary.DBModels.Task[]? Tasks)
        {


            var Errors = new List<TaskSaveInputErrorDTO>();

            // TODO: Is validate DAG
            var graph = new DirectedGraph(0);

            graph.LoadDataV1(Tasks);

            var isDAG = graph.IsDAG();
            if (isDAG == false)
            {
                Errors.Add(new TaskSaveInputErrorDTO()
                {
                    Messages = PrecedenceIsCycleMessage
                });
            }

            if (Errors.Count != 0)
            {
                throw new NotSuitableInputException(Errors);
            }

            return true;

        }

        public async Task<EstimatedResultDTO> GetEstimateOverallWorkforce(int projectId)
        {
            try
            {
                // validate project id exited
                var jwt = new JWTManagerService(httpContext);
                var cloudId = jwt.GetCurrentCloudId();

                var projectInDB = await db.Projects.FirstOrDefaultAsync(p => p.Id == projectId && p.CloudId == cloudId) ??
                throw new NotFoundException($"Can not find project :{projectId}");

                var response = await apiMicro.Get($"/api/WorkforceEstimator/GetEstimateWorkforceOverall?projectId={projectId}");
                dynamic responseContent;

                if (response.IsSuccessStatusCode)
                {
                    responseContent = await response.Content.ReadFromJsonAsync<EstimatedResultDTO>();
                }

                else
                {
                    throw new Exception(response.StatusCode.ToString());
                }
                return responseContent;

            }
            catch (MicroServiceAPIException ex)
            {
                throw new Exception(ex.mircoserviceResponse);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
