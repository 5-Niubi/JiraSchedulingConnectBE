using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;

namespace ResourceAssignAdmin.Pages.Plan
{
    public class DetailsModel : PageModel
    {
        private readonly ModelLibrary.DBModels.WoTaasContext _context;

        public DetailsModel(ModelLibrary.DBModels.WoTaasContext context)
        {
            _context = context;
        }

        public PlanSubscription PlanSubscription { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.PlanSubscriptions == null)
            {
                return NotFound();
            }

            var plansubscription = await _context.PlanSubscriptions.FirstOrDefaultAsync(m => m.Id == id);
            if (plansubscription == null)
            {
                return NotFound();
            }
            else
            {
                PlanSubscription = plansubscription;
            }
            return Page();
        }
    }
}
