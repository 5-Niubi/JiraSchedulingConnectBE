using AlgorithmServiceServer;
using UtilsLibrary;
using JiraSchedulingConnectAppService.Services.Interfaces;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Thread;
using System.Dynamic;
using UtilsLibrary.Exceptions;

namespace JiraSchedulingConnectAppService.Services
{
    public class AlgorithmService : IAlgorithmService
    {
        private readonly IAPIMicroserviceService apiMicro;
        private readonly IThreadService threadService;

        public AlgorithmService(JiraDemoContext db,
            IHttpContextAccessor httpContextAccessor, IAPIMicroserviceService apiMicro,
            IThreadService threadService)
        {
            this.apiMicro = apiMicro;
            this.threadService = threadService;
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
    }
}
