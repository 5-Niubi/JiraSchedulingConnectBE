using Microsoft.AspNetCore.Authorization;
using ModelLibrary.DTOs.Algorithm;
using UtilsLibrary.Exceptions;
using static UtilsLibrary.Const;

namespace JiraSchedulingConnectAppService.Services.Authorization
{
    public class ProjectLimitRequirement : IAuthorizationRequirement
    {
        public int maxLimit
        {
            get; set;
        }
        public ProjectLimitRequirement(int maxLimit)
        {
            this.maxLimit = maxLimit;

        }
    }


    public class ProjectLimitHandler : AuthorizationHandler<ProjectLimitRequirement, UserUsage>
    {


        private readonly HttpContext? httpContext;



        public ProjectLimitHandler(IHttpContextAccessor httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext;

        }

        protected override System.Threading.Tasks.Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ProjectLimitRequirement requirement,
            UserUsage resource
            )
        {

            if (resource.Plan != SUBSCRIPTION.PLAN_FREE)
            {
                context.Succeed(requirement);
            }


            else if (resource.ProjectActiveUsage < requirement.maxLimit)
            {
                context.Succeed(requirement);
            }

            else
            {
                throw new UnAuthorizedException($"You have create {resource.ProjectActiveUsage}. With Free Plan Only can create maximum {requirement.maxLimit} new project");

            }



            return System.Threading.Tasks.Task.CompletedTask;

        }


    }

}

