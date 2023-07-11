using ModelLibrary.DTOs.Algorithm;

namespace AlgorithmServiceServer.Services.Interfaces
{
    public interface IAccessDataToComputeService
    {
        public Task<List<ScheduleResultSolutionDTO>> GetDataToCompute(int parameterId);
    }
}
