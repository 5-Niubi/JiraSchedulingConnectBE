  using AlgorithmServiceServer;
using ModelLibrary.DTOs.PertSchedule;
using ModelLibrary.DTOs.Thread;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IAlgorithmService
    {
        public ThreadStartDTO TestConverter(int parameterId);
        public Task<EstimatedResultDTO> EstimateWorkforce(int projectId);

        

        
    }
}
