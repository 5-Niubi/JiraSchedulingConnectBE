using ModelLibrary.DTOs.Permission;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IPermissionService
    {
        public Task<PlanPermissionResponseDTO> AttachPlanPermission(AttachPlanPermissionRequestDTO AttachPermissionPlanRequest);
    }
}

