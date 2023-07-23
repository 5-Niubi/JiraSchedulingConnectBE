using ModelLibrary.DTOs.Subscriptions;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface ISubscriptionService
    {
        public Task<SubscriptionResDTO> GetCurrentSubscription();
    }
}
