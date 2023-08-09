using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using ModelLibrary.DTOs.Algorithm;
using UtilsLibrary.Exceptions;
using static UtilsLibrary.Const;

namespace JiraSchedulingConnectAppService.Services.Authorization
{


    public class ScheduleLimitRequirement : IAuthorizationRequirement
    {
        public int maxDailyLimit
        {
            get; set;
        }
        public ScheduleLimitRequirement(int maxDailyLimit)
        {
            this.maxDailyLimit = maxDailyLimit;

        }
    }


    public class ScheduleLimitHandler : AuthorizationHandler<ScheduleLimitRequirement, UserUsage>
    {


        private readonly IMapper mapper;
        private readonly HttpContext? httpContext;





        public ScheduleLimitHandler(
            IHttpContextAccessor httpContextAccessor
            )
        {
            mapper = mapper;
            httpContext = httpContextAccessor.HttpContext;



        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScheduleLimitRequirement requirement, UserUsage resource)
        {

            if (resource.Plan != SUBSCRIPTION.PLAN_FREE)
            {
                context.Succeed(requirement);
            }


            else if (resource.ScheduleUsage < requirement.maxDailyLimit)
            {
                context.Succeed(requirement);
            }

            else
            {
                throw new UnAuthorizedException($"You have schedule {resource.ScheduleUsage}. With Free Plan Only can schedule {requirement.maxDailyLimit} each day");

            }


            return Task.CompletedTask;



        }
    }
}

