using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;

namespace ResourceAssignAdmin.Pages.Upgrade
{
    public class IndexModel : PageModel
    {
        private readonly JiraDemoContext _context;

        public IndexModel(JiraDemoContext context)
        {
            this._context = context;
        }

        [BindProperty]
        public List<PlanSubscription> PlanSubscriptions { get; set; } = default!;

        public async Task<IActionResult> OnGet()
        {
            var planSubscription = await _context.PlanSubscriptions.ToListAsync();
            PlanSubscriptions = planSubscription;

            return Page();
        }
    }
}
