using System;
using AutoMapper;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using org.sqlite.core;
using static com.sun.xml.@internal.rngom.digested.DDataPattern;

namespace JiraSchedulingConnectAppService.Services.Policy
{


    public class ScheduleLimitRequirement : IAuthorizationRequirement
    {
        public int maxMonthlyLimit { get; set; }
        public ScheduleLimitRequirement(int maxMonthlyLimit)
		{
            maxMonthlyLimit = maxMonthlyLimit;

        }
	}


    public class ScheduleLimitHandler : AuthorizationHandler<ScheduleLimitRequirement>
    {

     
        private readonly IMapper mapper;
        private readonly HttpContext? httpContext;

        private readonly IScheduleService _scheduleService;
        private readonly ISubscriptionService _subscriptionService;
        


        public ScheduleLimitHandler(
            IHttpContextAccessor httpContextAccessor,
            IScheduleService scheduleService, ISubscriptionService subscriptionService)
        {
            mapper = mapper;
            httpContext = httpContextAccessor.HttpContext;
            _scheduleService = scheduleService;
            _subscriptionService = subscriptionService;
        }

        protected override async System.Threading.Tasks.Task HandleRequirementAsync(AuthorizationHandlerContext context, ScheduleLimitRequirement requirement)
        {

            // TODO: Get r
            var cloudId = context.User.Claims.FirstOrDefault(c => c.Type == "cloud_id").Value;

            // get Role
            var subcription = await _subscriptionService.GetCurrentSubscription();

            if (subcription.Plan.Name == "Premium")
            {
                context.Succeed(requirement);
                return ;
            }
       
            int monthlyUsage = await _scheduleService.GetScheduleMonthlyUsage();

            if (monthlyUsage < requirement.maxMonthlyLimit)
            {
                context.Succeed(requirement);
            }

            return;
           
        }
    }
}

