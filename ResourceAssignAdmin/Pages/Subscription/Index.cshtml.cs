using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
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

        public IList<ModelLibrary.DBModels.Subscription> Subscription { get;set; } = default!;

        public async System.Threading.Tasks.Task OnGetAsync(string? token, int? plan, int pageNum )
        {
            if (_context.Subscriptions != null)
            {
                var query =  _context.Subscriptions
                .Include(s => s.AtlassianToken)
                .Include(s => s.Plan)
                .Where(s => (
                    (token == null || s.Token == token)
                    &&
                    (plan == null || s.PlanId == plan)
                 ));

                (query, var totalPage, pageNum, var totalRecord) 
                    = Utils.MyQuery<ModelLibrary.DBModels.Subscription>.Paging(query, pageNum);

                Subscription = await query.ToListAsync();

                ViewData["PlanId"] = await _context.PlanSubscriptions.ToListAsync();

                ViewData["Token"] = token;
                ViewData["Plan"] = plan;
                ViewData["totalPage"] = totalPage;
                ViewData["currentPage"] = pageNum;
            }
        }
    }
}
