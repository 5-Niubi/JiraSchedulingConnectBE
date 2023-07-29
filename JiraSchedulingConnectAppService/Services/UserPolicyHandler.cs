using System;
using System.Security.Claims;
using AutoMapper;
using HeimGuard;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;

namespace JiraSchedulingConnectAppService.Services
{
    public class Permission
    {
        public string Name { get; set; }
        public List<string> Roles { get; set; }
    }

    public static class DummyPermissionStore
    {
        public static List<Permission> GetPermissions()
        {
            return new List<Permission>()
        {
            new()
            {
                Name = "RecipesFullAccess",
                Roles = new List<string>() { "chef" }
            },

             new()
            {
                Name = "GetABC",
                Roles = new List<string>() { "chef" }
            },

            new()
            {
                Name = "FUCK",
                Roles = new List<string>() { "chef" }
            }
        };
        }
    }

    public class UserPolicyHandler : IUserPolicyHandler
    {
        private readonly JiraDemoContext db;
        private readonly HttpContext? httpContext;

        public UserPolicyHandler( JiraDemoContext dbContext, IHttpContextAccessor httpContextAccessor)
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


            var roles = new List<string>()
            {
                "chef"
            };

            // this gets their permissions based on their roles. in this example, it's just using a static list
            var permissions = DummyPermissionStore.GetPermissions()
                .Where(p => p.Roles.Any(r => roles.Contains(r)))
                .Select(p => p.Name)
                .ToArray();

            return await System.Threading.Tasks.Task.FromResult(permissions.Distinct());
        }

        
	}
}

