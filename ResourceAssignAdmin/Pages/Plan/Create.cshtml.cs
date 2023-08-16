using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelLibrary.DBModels;

namespace ResourceAssignAdmin.Pages.Plan
{
    public class CreateModel : PageModel
    {
        private readonly ModelLibrary.DBModels.WoTaasContext _context;

        public CreateModel(ModelLibrary.DBModels.WoTaasContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public PlanSubscription PlanSubscription { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.PlanSubscriptions == null || PlanSubscription == null)
            {
                return Page();
            }

            _context.PlanSubscriptions.Add(PlanSubscription);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
