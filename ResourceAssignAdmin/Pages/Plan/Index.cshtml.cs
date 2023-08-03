using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;

namespace ResourceAssignAdmin.Pages.Plan
{
    public class IndexModel : PageModel
    {
        private readonly ModelLibrary.DBModels.JiraDemoContext _context;

        public IndexModel(ModelLibrary.DBModels.JiraDemoContext context)
        {
            _context = context;
        }

        public IList<PlanSubscription> PlanSubscription { get; set; } = default!;

        public async System.Threading.Tasks.Task OnGetAsync()
        {
            if (_context.PlanSubscriptions != null)
            {
                PlanSubscription = await _context.PlanSubscriptions.ToListAsync();
            }
        }
    }
}
