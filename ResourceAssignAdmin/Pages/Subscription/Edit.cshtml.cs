using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ResourceAssignAdmin.Pages.Subscription
{
    public class EditModel : PageModel
    {
        private readonly ModelLibrary.DBModels.JiraDemoContext _context;

        public EditModel(ModelLibrary.DBModels.JiraDemoContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ModelLibrary.DBModels.Subscription Subscription { get; set; } = default!;

        public async Task<IActionResult> PrepareView(int? id)
        {
            if (id == null || _context.Subscriptions == null)
            {
                return NotFound();
            }
            if (Subscription == null)
            {
                var subscription = await _context.Subscriptions
                    .Include(s => s.AtlassianToken).Include(s => s.Plan)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (subscription == null)
                {
                    return NotFound();
                }
                Subscription = subscription;
            }

            ViewData["UserToken"] = Subscription.AtlassianToken.UserToken;
            return Page();
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            return await PrepareView(id);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return await PrepareView(Subscription.Id);
                }
                await _context.Database.BeginTransactionAsync();

                // Get subscription that update to
                var updateSubscription = await _context.Subscriptions.OrderByDescending(s => s.CreateDatetime)
                    .FirstOrDefaultAsync(s => s.Id == Subscription.Id);
                /*
                // Validate Unique Toke
                var otherAtlasTokHaveSameUserTok = await _context.AtlassianTokens.FirstOrDefaultAsync(
                    at => at.UserToken == Subscription.AtlassianToken.UserToken
                    && at.Id != updateSubscription.AtlassianTokenId);
                if (otherAtlasTokHaveSameUserTok != null)
                {
                    ViewData["tokenMsg"] = "Token does exist";
                    return await PrepareView(Subscription.Id);
                }

                var updateAtlasTok = await _context.AtlassianTokens.FirstOrDefaultAsync(at =>
                    at.Id == updateSubscription.AtlassianTokenId);
                updateAtlasTok.UserToken = Subscription.AtlassianToken.UserToken;
                _context.AtlassianTokens.Update(updateAtlasTok);
                */
                // Remove Atlassian token after update it done
                Subscription.AtlassianToken = null;

                updateSubscription.CurrentPeriodStart = Subscription.CurrentPeriodStart;
                updateSubscription.CurrentPeriodEnd = Subscription.CurrentPeriodEnd;
                _context.Subscriptions.Update(updateSubscription);

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

            return RedirectToPage("./Index");
        }

    }
}
