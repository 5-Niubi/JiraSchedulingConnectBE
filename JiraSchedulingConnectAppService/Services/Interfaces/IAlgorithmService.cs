using AlgorithmServiceServer;
using ModelLibrary.DTOs.AlgorithmController;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IAlgorithmService
    {
        public Task<string> TestConverter(int projectId);
        public Task<EstimatedResultDTO> EstimateWorkforce(int projectId);
    }
}
