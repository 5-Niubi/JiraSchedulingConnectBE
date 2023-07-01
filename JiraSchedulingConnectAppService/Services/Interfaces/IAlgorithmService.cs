using ModelLibrary.DTOs.AlgorithmController;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IAlgorithmService
    {
        public Task<OutputFromORDTO> TestConverter(int projectId);
    }
}
