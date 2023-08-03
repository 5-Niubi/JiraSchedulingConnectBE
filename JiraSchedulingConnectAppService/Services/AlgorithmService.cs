using AlgorithmServiceServer;
using UtilsLibrary;
using JiraSchedulingConnectAppService.Services.Interfaces;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Thread;
using System.Dynamic;
using UtilsLibrary.Exceptions;
using ModelLibrary.DTOs;
using org.sqlite.core;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using ModelLibrary.DTOs.Invalidation;
using ModelLibrary.DTOs.PertSchedule;
using RcpspAlgorithmLibrary;

namespace JiraSchedulingConnectAppService.Services
{
    public class AlgorithmService : IAlgorithmService
    {
        private readonly IAPIMicroserviceService apiMicro;
        private readonly IThreadService threadService;
        private readonly IAuthorizationService _authorizationService;

        private readonly JiraDemoContext db;
        private readonly HttpContext? httpContext;

        public const string PrecedenceIsCycleMessage = "Tasks be cycle!";


        public AlgorithmService(
            JiraDemoContext db,
            IAuthorizationService _authorizationService,
            IHttpContextAccessor httpContextAccessor,
            IAPIMicroserviceService apiMicro,
            IThreadService threadService)
        {
            this.apiMicro = apiMicro;
            this.threadService = threadService;
            this.db = db;
            this.httpContext = httpContextAccessor.HttpContext;
            this._authorizationService = _authorizationService;
        }





        public async System.Threading.Tasks.Task IsValidExecuteAuthorize()
        {

            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            var planId = await db.Subscriptions.Include(s => s.AtlassianToken)
                .Include(s => s.Plan)
                .Where(s => s.AtlassianToken.CloudId == cloudId && s.CancelAt == null)
                .Select(S => S.PlanId).FirstOrDefaultAsync();

            int scheduleMonthlyUsage = await GetScheduleMonthlyUsage();

            await _authorizationService.AuthorizeAsync(httpContext.User, new ModelLibrary.DTOs.Algorithm.UserUsage()
            {
                Plan = (int)planId,
                ScheduleUsage = scheduleMonthlyUsage

            }, "LimitedScheduleTimeByMonth");
        }


        public async Task<int> GetScheduleMonthlyUsage()
        {
         
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();


            var ProjectIds = await db.Projects.Where(pr => pr.CloudId == cloudId).Select(p => p.Id).ToArrayAsync();

            DateTime currentMonthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime currentMonthEnd = currentMonthStart.AddMonths(1).AddTicks(-1);

            var MonthlyUsage = await db.Parameters
                .Where(pr => ProjectIds.Contains(pr.Id) && pr.CreateDatetime >= currentMonthStart && pr.CreateDatetime <= currentMonthEnd).Distinct()
                .CountAsync();


            return MonthlyUsage;
        
            
        }



        public   ThreadStartDTO ExecuteAlgorithm(int parameterId)
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
                }
                catch (NotFoundException ex)
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
                }
            }
            catch {/* Do nothing*/ }
        }


        public async Task<EstimatedResultDTO> EstimateWorkforce(int projectId)
        {
            

            try
            {

                // validate project id exited
                var jwt = new JWTManagerService(httpContext);
                var cloudId = jwt.GetCurrentCloudId();

                var projectInDB = await db.Projects.FirstOrDefaultAsync(p => p.Id == projectId && p.CloudId == cloudId) ??
                throw new NotFoundException($"Can not find project :{projectId}");


                // get list task by project
                var TaskList = await db.Tasks.Include(s => s.TaskPrecedenceTasks).Where(t => t.ProjectId == projectId && t.IsDelete == false).ToArrayAsync();
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

            }
            catch(MicroServiceAPIException ex)
            {
                throw new Exception(ex.mircoserviceResponse);

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            



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
