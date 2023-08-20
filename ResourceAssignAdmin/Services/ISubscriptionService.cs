using ModelLibrary.DBModels;

namespace ResourceAssignAdmin.Services
{
    public interface ISubscriptionService
    {
        public System.Threading.Tasks.Task CreatePlan(string userToken,
            ModelLibrary.DBModels.Subscription subscription);
        public Task<PlanSubscription> CurrentSubscriptionPlan(string userToken);
        public bool IsPlusPlan(string userToken);
    }
}
