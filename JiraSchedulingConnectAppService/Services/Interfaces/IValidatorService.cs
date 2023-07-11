using System;
using ModelLibrary.DTOs.PertSchedule;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
	public interface IValidatorService
	{
        public Task<bool> IsValidDAG(int projectId);

        public Task<bool> IsValidRequiredParameters(ParameterRequest parameterRequest );
    }
}

