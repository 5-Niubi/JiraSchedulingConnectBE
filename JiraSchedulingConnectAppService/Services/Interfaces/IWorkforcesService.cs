using ModelLibrary.DTOs.Parameters;
using ModelLibrary.DTOs.Workforce;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IWorkforcesService
    {
        public Task<List<WorkforceDTOResponse>> GetAllWorkforces(List<int> workforceIds);
        public Task<List<WorkforceViewDTOResponse>> GetWorkforceScheduleByProject();
        public Task<WorkforceDTOResponse> CreateWorkforce(WorkforceRequestDTO w);
        public Task<WorkforceDTOResponse> GetWorkforceById(string workforce_id);
        public Task<bool> DeleteWorkforce(int workforce_id);
        public Task<WorkforceDTOResponse> UpdateWorkforce(WorkforceRequestDTO w);


    }
}

