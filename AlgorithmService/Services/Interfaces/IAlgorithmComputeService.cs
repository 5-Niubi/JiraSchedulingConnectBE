using ModelLibrary.DTOs.Algorithm;

namespace AlgorithmServiceServer.Services.Interfaces
{
    public interface IAlgorithmComputeService
    {
        public Task<List<ScheduleResultSolutionDTO>> GetDataToCompute(int parameterId);
    }
}
