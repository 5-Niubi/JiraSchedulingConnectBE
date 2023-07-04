using ModelLibrary.DBModels;
using ModelLibrary.DTOs;
using Task = System.Threading.Tasks.Task;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IWorkforcesService
    {
        public Task<List<Workforce>> GetAllWorkforces();
        public Task<WorkforceDTO> CreateWorkforce(WorkforceDTO w);
        public Task<Workforce> GetWorkforceById(string workforce_id);
        public Task DeleteWorkforce(Workforce w);
        public Task<WorkforceDTO> UpdateWorkforce(WorkforceDTO w);
    }
}

