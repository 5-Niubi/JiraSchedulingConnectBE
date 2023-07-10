using ModelLibrary.DBModels;
using ModelLibrary.DTOs;
using ModelLibrary.DTOs.Parameters;
using Task = System.Threading.Tasks.Task;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
	public interface IWorkforcesService
	{
		public Task<List<WorkforceDTOResponse>> GetAllWorkforces();
		public Task<WorkforceDTOResponse> CreateWorkforce(WorkforceDTORequest w);
		public Task<WorkforceDTOResponse> GetWorkforceById(string workforce_id);
		public Task<WorkforceDTOResponse> DeleteWorkforce(string workforce_id);
		public Task<WorkforceDTOResponse> UpdateWorkforce(WorkforceDTORequest w);
	}
}

