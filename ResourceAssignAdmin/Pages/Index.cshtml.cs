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

        public async Task<IActionResult> OnGet(int? orderyear)
        {
            if (orderyear == null)
            {
                orderyear = DateTime.Now.Year;
            }

            var activeSubscription = _context.Subscriptions.Where(s => s.CancelAt == null);
            var totalFreeUsers = await activeSubscription
                .Where(a => a.PlanId == Const.SUBSCRIPTION.PLAN_FREE).CountAsync();

            var totalPlusUser = await activeSubscription
                .Where(a => a.PlanId == Const.SUBSCRIPTION.PLAN_PLUS).CountAsync();

            var userIn12Months = (from user in _context.AtlassianTokens
                                  where user.CreateDatetime.Value.Year == DateTime.Now.Year
                                  group user by user.CreateDatetime.Value.Month into userJoinMonth
                                  orderby userJoinMonth.Key ascending
                                  select new { Month = userJoinMonth.Key, Users = userJoinMonth.Select(u => u.Id).Count() }
                                  ).ToDictionary(e => e.Month, e => e.Users);
            var listUserIn12Months = new List<int>();
            for (var i = 1; i <= 12; i++)
            {
                if (!userIn12Months.Keys.Contains(i))
                {
                    userIn12Months.Add(i, 0);
                }
                listUserIn12Months.Add(userIn12Months[i]);
            }

            var userPreIn12Months = (from subs in _context.Subscriptions
                                     where subs.CreateDatetime.Value.Year == DateTime.Now.Year
                                        && subs.PlanId == Const.SUBSCRIPTION.PLAN_PLUS
                                        && subs.CancelAt == null
                                     group subs by subs.CreateDatetime.Value.Month into userJoinMonth
                                     orderby userJoinMonth.Key ascending
                                     select new { Month = userJoinMonth.Key, Users = userJoinMonth.Select(u => u.Id).Count() }
                                  ).ToDictionary(e => e.Month, e => e.Users);

            var listUserPreIn12Months = new List<int>();
            for (var i = 1; i <= 12; i++)
            {
                if (!userPreIn12Months.Keys.Contains(i))
                {
                    userPreIn12Months.Add(i, 0);
                }
                listUserPreIn12Months.Add(userPreIn12Months[i]);
            }

            // Get all year
            var joinDate = _context.AtlassianTokens.GroupBy(u => u.CreateDatetime.Value.Year)
                .Select(group => group.First()).ToList();
            List<int> orderyearsList = new List<int>();
            joinDate.ForEach(e => orderyearsList.Add(e.CreateDatetime.Value.Year));

            ViewData["YearSelection"] = orderyearsList;
            int[] totalUserResult = { totalFreeUsers, totalPlusUser };
            ViewData["TotalUsers"] = JsonConvert.SerializeObject(totalUserResult);
            ViewData["UserIn12Months"] = JsonConvert.SerializeObject(listUserIn12Months);
            ViewData["UserPreIn12Months"] = JsonConvert.SerializeObject(listUserPreIn12Months);
            ViewData["SelectedYear"] = orderyear;
            return Page();
        }
    }
}