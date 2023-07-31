using System;
using ModelLibrary.DTOs.Parameters;
using ModelLibrary.DTOs.Permission;
using ModelLibrary.DTOs.PertSchedule;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
	public interface IPermissionService
	{
        public Task<PlanPermissionResponseDTO> AttachPlanPermission(AttachPlanPermissionRequestDTO AttachPermissionPlanRequest);
    }
}

