using System;
using Microsoft.AspNetCore.Authorization;

namespace JiraSchedulingConnectAppService.Services.Policy
{
	public class MaximumScheduleTimeRequirement : IAuthorizationRequirement
    {
        protected int MaximumTime { get; set; }



        public MaximumScheduleTimeRequirement(int maximumTime)
		{
            MaximumTime = maximumTime;

        }
	}
}

