using System;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using ModelLibrary.DBModels;
using org.sqlite.core;

namespace JiraSchedulingConnectAppService.Services.Policy
{
    public class MaximumScheduleHandler : AuthorizationHandler<MaximumScheduleTimeRequirement>
    {

        private readonly ModelLibrary.DBModels.JiraDemoContext db;
        private readonly IMapper mapper;
        private readonly HttpContext? httpContext;

        public MaximumScheduleHandler(JiraDemoContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            db = dbContext;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext;
        }

        protected override System.Threading.Tasks.Task HandleRequirementAsync(AuthorizationHandlerContext context, MaximumScheduleTimeRequirement requirement)
        {
            // get all task active by project id
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            if(cloudId == null)
            {
                context.Succeed(requirement);
            }

            return System.Threading.Tasks.Task.FromResult(0);
        }
    }
}

