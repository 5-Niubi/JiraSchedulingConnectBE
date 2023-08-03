using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;

namespace ResourceAssignAdmin.Pages.Plan
{
    public class DeleteModel : PageModel
    {
        private readonly ModelLibrary.DBModels.JiraDemoContext _context;

        public DeleteModel(ModelLibrary.DBModels.JiraDemoContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.PlanSubscriptions == null)
            {
                return NotFound();
            }
            var plansubscription = await _context.PlanSubscriptions.FindAsync(id);

            if (plansubscription != null)
            {
                PlanSubscription = plansubscription;
                _context.PlanSubscriptions.Remove(PlanSubscription);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
