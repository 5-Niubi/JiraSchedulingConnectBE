using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using UtilsLibrary;

namespace ResourceAssignAdmin.Pages.Subscription
{
    public class CreateModel : PageModel
    {
        private readonly ModelLibrary.DBModels.JiraDemoContext _context;

        public CreateModel(ModelLibrary.DBModels.JiraDemoContext context)
        {
            _context = context;
        }

        private IActionResult PrepareView()
        {
            ViewData["PlanId"] = new SelectList(_context.PlanSubscriptions, "Id", "Name");
            return Page();
        }

        public IActionResult OnGet()
        {
            return PrepareView();
        }

        [BindProperty]
        public ModelLibrary.DBModels.Subscription Subscription { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Subscriptions == null || Subscription == null)
            {
                return PrepareView();
            }

            await _context.Database.BeginTransactionAsync();
            try
            {
                // Find correct user with token
                var atlassianToken = await _context.AtlassianTokens
                    .FirstOrDefaultAsync(at => at.UserToken == Subscription.AtlassianToken.UserToken);
                 
                if (atlassianToken == null)
                {
                    ViewData["tokenMsg"] = "Invalid Token";
                    return PrepareView();
                }
                // Remove unecessary Atlassian token
                Subscription.AtlassianToken = null;

                // Find lastest subscription active
                var lastestSubscription = await _context.Subscriptions.OrderByDescending(s => s.CreateDatetime)
                    .FirstOrDefaultAsync(s => s.AtlassianTokenId == atlassianToken.Id);
                if (lastestSubscription == null)
                {
                    ViewData["tokenMsg"] = "Invalid Token";
                    return PrepareView();
                }

                if (lastestSubscription.PlanId == Const.SUBSCRIPTION.PLAN_FREE
                    && Subscription.PlanId == Const.SUBSCRIPTION.PLAN_FREE)
                {
                    ViewData["errorMsg"] = "Invalid Subscription. This user can only Upgrade to higher plan";
                    return PrepareView();
                }

                if (lastestSubscription.CancelAt == null)
                {
                    lastestSubscription.CancelAt = DateTime.Now;
                }

                Subscription.AtlassianTokenId = atlassianToken.Id;

                if (Subscription.PlanId == Const.SUBSCRIPTION.PLAN_PLUS)
                {
                    Subscription.CurrentPeriodEnd = Subscription.CurrentPeriodStart.Value.AddMonths(12);
                }
                else if (Subscription.PlanId == Const.SUBSCRIPTION.PLAN_FREE)
                {
                    Subscription.CurrentPeriodEnd = null;
                }
                _context.Subscriptions.Add(Subscription);

                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _context.Database.RollbackTransactionAsync();
                throw;
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
            return RedirectToPage("./Index");
        }
    }
}
