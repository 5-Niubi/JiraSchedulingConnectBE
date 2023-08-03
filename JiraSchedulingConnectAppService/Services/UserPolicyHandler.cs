using HeimGuard;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;

namespace JiraSchedulingConnectAppService.Services
{
    public class Permission
    {
        public string Name
        {
            get; set;
        }
        public List<string> Roles
        {
            get; set;
        }
    }

    public class UserPolicyHandler : IUserPolicyHandler
    {
        private readonly JiraDemoContext db;
        private readonly HttpContext? httpContext;

        public UserPolicyHandler(JiraDemoContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            this.db = dbContext;
            this.httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<IEnumerable<string>> GetUserPermissions()
        {
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            var subscription = await db.Subscriptions.Include(s => s.AtlassianToken)
                 .Include(s => s.Plan)
                 .Where(s => s.AtlassianToken.CloudId == cloudId && s.CancelAt == null)
                 .OrderByDescending(s => s.CreateDatetime)
                 .FirstOrDefaultAsync();

            // this gets their permissions based on their roles. in this example, it's just using a static list
            var permissions = await db.PlanPermissions
                .Where(p => p.PlanSubscriptionId == subscription.PlanId && p.IsDelete == false).Select(pr => pr.Permission).ToArrayAsync();

            return await System.Threading.Tasks.Task.FromResult(permissions.Distinct());
        }


    }
}

