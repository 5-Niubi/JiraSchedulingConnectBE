using System;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs;
using Task = System.Threading.Tasks.Task;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
	public interface IWorkforcesService
	{
		public Task<List<WorkforceDTO.Response>> GetAllWorkforces();
		public Task<WorkforceDTO.Response> CreateWorkforce(WorkforceDTO.Request w);
		public Task<WorkforceDTO.Request> GetWorkforceById(string workforce_id);
		public Task DeleteWorkforce(WorkforceDTO.Request w);
		public Task<WorkforceDTO.Response> UpdateWorkforce(WorkforceDTO.Request w);
	}
}

