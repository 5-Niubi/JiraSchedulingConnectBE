using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UtilsLibrary;

namespace ResourceAssignAdmin.Pages.Subscription
{
    public class IndexModel : PageModel
    {
        private readonly ModelLibrary.DBModels.JiraDemoContext _context;

        public IndexModel(ModelLibrary.DBModels.JiraDemoContext context)
        {
            _context = context;
        }

        public IList<ModelLibrary.DBModels.Subscription> Subscription { get; set; } = default!;

        public async System.Threading.Tasks.Task OnGetAsync(string? token, int? plan, int pageNum = 0, string status = "")
        {
            if (status == null)
            {
                status = string.Empty;
            }
            if (_context.Subscriptions != null)
            {
                var query = _context.Subscriptions
                .Include(s => s.AtlassianToken)
                .Include(s => s.Plan)
                .Where(s => (
                    (token == null || s.AtlassianToken.UserToken == token)
                    &&
                    (plan == null || s.PlanId == plan)
                    &&
                    (status == "all" ||
                        (status == "" && s.CancelAt == null) || (status == "deactive" && s.CancelAt != null))
                 ));

                (query, var totalPage, pageNum, var totalRecord)
                    = Utils.MyQuery<ModelLibrary.DBModels.Subscription>.Paging(query, pageNum);

                Subscription = await query.ToListAsync();

                ViewData["PlanId"] = await _context.PlanSubscriptions.ToListAsync();

                ViewData["Token"] = token;
                ViewData["Plan"] = plan;
                ViewData["totalPage"] = totalPage;
                ViewData["currentPage"] = pageNum;
                ViewData["status"] = status;
            }
        }
    }
}
