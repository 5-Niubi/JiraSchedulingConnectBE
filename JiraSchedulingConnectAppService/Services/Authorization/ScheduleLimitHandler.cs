using System;
using AutoMapper;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Algorithm;
using Newtonsoft.Json;
using org.sqlite.core;
using UtilsLibrary.Exceptions;
using static com.sun.xml.@internal.rngom.digested.DDataPattern;
using static UtilsLibrary.Const;

namespace JiraSchedulingConnectAppService.Services.Policy
{


    public class ScheduleLimitRequirement : IAuthorizationRequirement
    {
        public int maxMonthlyLimit { get; set; }
        public ScheduleLimitRequirement(int maxMonthlyLimit)
		{
            this.maxMonthlyLimit = maxMonthlyLimit;

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

        protected override  System.Threading.Tasks.Task HandleRequirementAsync(AuthorizationHandlerContext context, ScheduleLimitRequirement requirement, UserUsage resource)
        {

            if (resource.Plan != SUBSCRIPTION.PLAN_FREE)
            {
                context.Succeed(requirement);
            }


            else if (resource.ScheduleUsage < requirement.maxMonthlyLimit)
            {
                context.Succeed(requirement);
            }

            else
            {
                throw new UnAuthorizedException($"You have schedule {resource.ScheduleUsage}. With Free Plan Only can schedule {requirement.maxMonthlyLimit} each month");

            }


            return System.Threading.Tasks.Task.CompletedTask;



        }
    }
}

