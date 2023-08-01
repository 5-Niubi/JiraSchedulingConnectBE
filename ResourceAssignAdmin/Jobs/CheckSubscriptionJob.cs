using ModelLibrary.DBModels;
using Quartz;
using UtilsLibrary;

namespace ResourceAssignAdmin.Jobs
{
    public class CheckSubscriptionJob : IJob
    {
        public System.Threading.Tasks.Task Execute(IJobExecutionContext context)
        {
            var db = new JiraDemoContext();
            var outDateActiveSubscription = db.Subscriptions.Where(s => s.CancelAt == null
             && s.CurrentPeriodEnd.Value <= DateTime.Now
             && s.PlanId != Const.SUBSCRIPTION.PLAN_FREE).ToList();

            var newFreeSubscription = new List<Subscription>();
            foreach (var subscription in outDateActiveSubscription)
            {
                subscription.CancelAt = DateTime.Now;

                var freeSubs = new Subscription()
                {
                    AtlassianTokenId = subscription.AtlassianTokenId,
                    PlanId = Const.SUBSCRIPTION.PLAN_FREE,
                    CreateDatetime = DateTime.Now,
                    CurrentPeriodStart = DateTime.Now,
                };
                newFreeSubscription.Add(freeSubs);
            }
            db.Subscriptions.UpdateRange(outDateActiveSubscription);
            db.Subscriptions.AddRange(newFreeSubscription);
            db.SaveChanges();

            return System.Threading.Tasks.Task.FromResult(true);
        }
    }
}
