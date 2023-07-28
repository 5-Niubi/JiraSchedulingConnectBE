using Braintree;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using UtilsLibrary;
using UtilsLibrary.Exceptions;

namespace ResourceAssignAdmin.Services
{
    public class SubscriptionService : ISubscriptionService
    {

        private readonly JiraDemoContext _context;

        public SubscriptionService(JiraDemoContext context)
        {
            _context = context;
        }

        public async System.Threading.Tasks.Task CreatePlan(string userToken,
            ModelLibrary.DBModels.Subscription subscription)
        {
            await _context.Database.BeginTransactionAsync();
            try
            {
                // Find correct user with token
                var atlassianToken = await _context.AtlassianTokens
                    .FirstOrDefaultAsync(at => at.UserToken == userToken);

                if (atlassianToken == null)
                {
                    throw new NotFoundException("Invalid Token");
                }

                // Find lastest subscription active
                var lastestSubscription = await _context.Subscriptions.OrderByDescending(s => s.CreateDatetime)
                    .FirstOrDefaultAsync(s => s.AtlassianTokenId == atlassianToken.Id);
                if (lastestSubscription == null)
                {
                    throw new NotFoundException("Invalid Token");
                }

                if (lastestSubscription.PlanId == Const.SUBSCRIPTION.PLAN_FREE
                    && subscription.PlanId == Const.SUBSCRIPTION.PLAN_FREE)
                {
                    throw new NotSuitableInputException
                        ("Invalid Subscription.This user can only Upgrade to higher plan");
                }

                if (lastestSubscription.CancelAt == null)
                {
                    lastestSubscription.CancelAt = DateTime.Now;
                }

                subscription.AtlassianTokenId = atlassianToken.Id;

                if (subscription.PlanId == Const.SUBSCRIPTION.PLAN_PLUS)
                {
                    subscription.CurrentPeriodEnd = subscription.CurrentPeriodStart.Value.AddMonths(12);
                }
                else if (subscription.PlanId == Const.SUBSCRIPTION.PLAN_FREE)
                {
                    subscription.CurrentPeriodEnd = null;
                }
                _context.Subscriptions.Add(subscription);

                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();
            }
            catch (Exception)
            {
                await _context.Database.RollbackTransactionAsync();
                throw;
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }

        public async Task<PlanSubscription> CurrentSubscriptionPlan(string userToken)
        {
            // Find lastest subscription active
            var lastestSubscription = await _context.Subscriptions
                .Include(s => s.AtlassianToken)
                .Include(s => s.Plan)
                .OrderByDescending(s => s.CreateDatetime)
                .FirstOrDefaultAsync(s => s.AtlassianToken.UserToken == userToken && s.CancelAt == null);
            if (lastestSubscription == null)
            {
                throw new NotFoundException("Invalid Token");
            }

            return lastestSubscription.Plan;
        }

        public bool IsPlusPlan(string userToken)
        {
            return CurrentSubscriptionPlan(userToken).Result.Id
                == Const.SUBSCRIPTION.PLAN_PLUS;
        }
    }
}
