using AlgorithmServiceServer;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IAlgorithmService
    {
        public Task<string> TestConverter(int projectId, int parameterId);
        public Task<EstimatedResultDTO> EstimateWorkforce(int projectId);
    }
}
