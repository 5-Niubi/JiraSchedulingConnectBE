using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using Newtonsoft.Json;
using UtilsLibrary;

namespace ResourceAssignAdmin.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly JiraDemoContext _context;

        public IndexModel(ILogger<IndexModel> logger, JiraDemoContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> OnGet()
        {
            var activeSubscription = _context.Subscriptions.Where(s => s.CancelAt == null);
            var totalFreeUsers = await activeSubscription
                .Where(a => a.PlanId == Const.SUBSCRIPTION.PLAN_FREE).CountAsync();

            var totalPlusUser = await activeSubscription
                .Where(a => a.PlanId == Const.SUBSCRIPTION.PLAN_PLUS).CountAsync();
            int[] totalUserResult = {  totalFreeUsers, totalPlusUser  };
            ViewData["TotalUsers"] =  JsonConvert.SerializeObject(totalUserResult);

            return Page();
        }
    }
}