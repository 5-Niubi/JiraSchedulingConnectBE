using System;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using org.sqlite.core;
using static com.sun.xml.@internal.rngom.digested.DDataPattern;

namespace JiraSchedulingConnectAppService.Services.Policy
{


    public class ScheduleTimeRequirement : IAuthorizationRequirement
    {
        public int Number { get; set; }
        public ScheduleTimeRequirement(int Number)
		{
            Number = Number;

        }
	}


    public class ScheduleTimeRequirementHandler : AuthorizationHandler<ScheduleTimeRequirement>
    {

        //TODO: Note: Chưa thiết kế bảng Lưu trữ permission properties của từng plan nên phải hardcode
        private const int FREE_PLAN_ID = 1; 
        private const int FREE_PLAN_MAXIMUM_SAVE_PARAMS = 3; 

        private readonly ModelLibrary.DBModels.JiraDemoContext db;
        private readonly IMapper mapper;
        private readonly HttpContext? httpContext;

        public ScheduleTimeRequirementHandler(JiraDemoContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            db = dbContext;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext;
        }

        protected override async System.Threading.Tasks.Task HandleRequirementAsync(AuthorizationHandlerContext context, ScheduleTimeRequirement requirement)
        {
            // get all task active by project id
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            if (!context.User.HasClaim(x => x.Type == "ScheduleTime"))
            {
                return;
            }

            int scheduleTime = int.Parse(context.User.FindFirst(x => x.Type == "ScheduleTime").Value);

            //// TODO: Check Permission User's used schetime times by month
            //var subscription = await db.Subscriptions.Include(s => s.AtlassianToken)
            //     .Include(s => s.Plan)
            //     .Where(s => s.AtlassianToken.CloudId == cloudId && s.CancelAt == null)
            //     .OrderByDescending(s => s.CreateDatetime)
            //     .FirstOrDefaultAsync();

            //DateTime currentMonthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //DateTime currentMonthEnd = currentMonthStart.AddMonths(1).AddTicks(-1);

            if (scheduleTime < requirement.Number)
            {
                context.Succeed(requirement);
            }

            return;
            //if (subscription.PlanId == FREE_PLAN_ID) // free plan
            //{
            //    var ProjectIds = await db.Projects.Where(pr => pr.CloudId == cloudId).Select(p => p.Id).ToArrayAsync();

            //    var countSaveParam = await db.Parameters
            //        .Where(pr => ProjectIds.Contains(pr.Id) && pr.CreateDatetime >= currentMonthStart && pr.CreateDatetime <= currentMonthEnd).Distinct()
            //        .CountAsync();

            //    if(countSaveParam > FREE_PLAN_MAXIMUM_SAVE_PARAMS)
            //    {
            //        return;
            //    }
            //}

            //context.Succeed(requirement);
            //return;


        }
    }
}

