using AutoMapper;
using JiraSchedulingConnectAppService.Services.Interfaces;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Permission;
using UtilsLibrary.Exceptions;

namespace JiraSchedulingConnectAppService.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly WoTaasContext db;
        private readonly IMapper mapper;
        private readonly HttpContext? httpContext;

        private const string NotFoundPlanMessage = "Not found Plan!";
        private const string NotFoundPermissionMessage = "Not found Permission!";
        private const string AttachNotValidMessage = "Attach Not validate!";

        public PermissionService(WoTaasContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            db = dbContext;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<PlanPermissionResponseDTO> AttachPlanPermission(AttachPlanPermissionRequestDTO AttachPermissionPlanRequest)
        {

            // validate plan exited
            var planExited = db.PlanSubscriptions.Where(ps => ps.Id == AttachPermissionPlanRequest.PlanId && ps.IsDelete == false).FirstOrDefault();

            if (planExited == null)
            {
                throw new NotSuitableInputException(NotFoundPlanMessage);
            }

            // TODO: Validate permission name exited on system

            // validate plan and permission were not attached before
            var attachedExited = db.PlanPermissions.Where(ps => ps.PlanSubscriptionId == AttachPermissionPlanRequest.PlanId
            && ps.Permission.Trim().ToLower() == AttachPermissionPlanRequest.PermissionName.Trim().ToLower()
            && ps.IsDelete == false)
                .FirstOrDefault();

            if (attachedExited != null)
            {
                throw new NotSuitableInputException(AttachNotValidMessage);
            }

            // insert
            var PermissionPlan = new PlanPermission()
            {
                Permission = AttachPermissionPlanRequest.PermissionName.Trim(),
                PlanSubscriptionId = AttachPermissionPlanRequest.PlanId,
                IsDelete = false

            };

            var entity = db.PlanPermissions.Add(PermissionPlan);
            await db.SaveChangesAsync();

            // mapping response
            var response = mapper.Map<PlanPermissionResponseDTO>(entity.Entity);
            return response;
        }
    }
}

