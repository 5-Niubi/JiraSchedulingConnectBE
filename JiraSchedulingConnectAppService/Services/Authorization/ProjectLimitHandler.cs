using System;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace JiraSchedulingConnectAppService.Services.Authorization
{
	public class ProjectLimitRequirement : IAuthorizationRequirement
    {
        public int maxLimit { get; set; }
        public ProjectLimitRequirement(int maxLimit)
		{
            maxLimit = maxLimit;

        }
	}


    public class ProjectLimitHandler : AuthorizationHandler<ProjectLimitRequirement>
    {


        private readonly HttpContext? httpContext;
        private readonly IProjectServices _projectService;
        private readonly ISubscriptionService _subscriptionService;


        public ProjectLimitHandler( IHttpContextAccessor httpContextAccessor,
            IProjectServices ProjectService, ISubscriptionService SubscriptionService)
        {
            httpContext = httpContextAccessor.HttpContext;
            _projectService = ProjectService;
            _subscriptionService = SubscriptionService;
        }

        protected override async System.Threading.Tasks.Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectLimitRequirement requirement)
        {

            // TODO: Get r
            var cloudId = context.User.Claims.FirstOrDefault(c => c.Type == "cloud_id").Value;

            // get Role
            var subcription = await _subscriptionService.GetCurrentSubscription();

            if (subcription.Plan.Name == "Premium")
            {
                context.Succeed(requirement);
                return;
            }

            int createdNumber = await _projectService.GetCreatedProjectNumber();

            if (createdNumber < requirement.maxLimit)
            {
                context.Succeed(requirement);
            }

            return;

        }
    }

}

