using AlgorithmServiceServer;
using ModelLibrary.DTOs.Thread;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IAlgorithmService
    {
        public ThreadStartDTO ExecuteAlgorithm(int parameterId);
        public Task<EstimatedResultDTO> EstimateWorkforce(int projectId);
        public Task<EstimatedResultDTO> GetEstimateOverallWorkforce(int projectId);
    }
}
