using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;

namespace ResourceAssignAdmin.Pages.Plan
{
    public class EditModel : PageModel
    {
        private readonly ModelLibrary.DBModels.JiraDemoContext _context;

        public EditModel(ModelLibrary.DBModels.JiraDemoContext context)
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

            var plansubscription =  await _context.PlanSubscriptions.FirstOrDefaultAsync(m => m.Id == id);
            if (plansubscription == null)
            {
                return NotFound();
            }
            PlanSubscription = plansubscription;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(PlanSubscription).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlanSubscriptionExists(PlanSubscription.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool PlanSubscriptionExists(int id)
        {
          return (_context.PlanSubscriptions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
