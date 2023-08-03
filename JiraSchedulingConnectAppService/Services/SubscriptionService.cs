using AutoMapper;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Subscriptions;
using UtilsLibrary.Exceptions;

namespace JiraSchedulingConnectAppService.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly JiraDemoContext db;
        private readonly HttpContext http;
        private readonly IMapper mapper;

        public SubscriptionService(JiraDemoContext db,
            IHttpContextAccessor httpAccess, IMapper mapper) {
            this.db = db;
            http = httpAccess.HttpContext;
            this.mapper = mapper;
        }

        public async Task<SubscriptionResDTO> GetCurrentSubscription()
        {
            var cloudId = new JWTManagerService(http).GetCurrentCloudId();
            var subscription = await db.Subscriptions.Include(s => s.AtlassianToken)
                .Include(s => s.Plan)
                .Where(s => s.AtlassianToken.CloudId == cloudId && s.CancelAt == null)
                .OrderByDescending(s => s.CreateDatetime)
                .FirstOrDefaultAsync();
            if (subscription == null)
            {
                throw new NotFoundException();
            }
            var subscriptionResDTO = mapper.Map<SubscriptionResDTO>(subscription);
            return subscriptionResDTO;
        }
    }
}
