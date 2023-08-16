using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ResourceAssignAdmin.Services;

namespace ResourceAssignAdmin.Pages.Upgrade
{
    public class IndexModel : PageModel
    {
        private readonly WoTaasContext _context;
        private readonly ISubscriptionService _subscriptionService;

        public IndexModel(WoTaasContext context,
            ISubscriptionService subscription)
        {
            _context = context;
            _subscriptionService = subscription;
        }

        [BindProperty]
        public List<PlanSubscription> PlanSubscriptions { get; set; } = default!;

        public async Task<IActionResult> OnGet(string? token = "")
        {
            if (await _context.AtlassianTokens.FirstOrDefaultAsync(at => at.UserToken == token) == null)
            {
                return NotFound();
            }
            var planSubscription = await _context.PlanSubscriptions.ToListAsync();
            PlanSubscriptions = planSubscription;
            ViewData["UserToken"] = token;
            ViewData["CurrentSubscription"] = await _subscriptionService.CurrentSubscriptionPlan(token);
            return Page();
        }
    }
}
