using ModelLibrary.DTOs.AlgorithmController;

namespace AlgorithmServiceServer.Services.Interfaces
{
    public interface IAccessDataToComputeService
    {
        public Task<List<OutputFromORDTO>> GetDataToCompute(int projectId);
    }
}
