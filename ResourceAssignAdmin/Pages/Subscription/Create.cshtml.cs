using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Braintree.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ResourceAssignAdmin.Services;
using UtilsLibrary;
using UtilsLibrary.Exceptions;

namespace ResourceAssignAdmin.Pages.Subscription
{
    public class CreateModel : PageModel
    {
        private readonly ModelLibrary.DBModels.JiraDemoContext _context;
        private readonly ISubscriptionService subscriptionService;

        public CreateModel(ModelLibrary.DBModels.JiraDemoContext context,
            ISubscriptionService subscriptionService)
        {
            _context = context;
            this.subscriptionService = subscriptionService;
        }

        private IActionResult PrepareView()
        {
            ViewData["PlanId"] = new SelectList(_context.PlanSubscriptions, "Id", "Name");
            return Page();
        }

        public IActionResult OnGet()
        {
            return PrepareView();
        }

        [BindProperty]
        public ModelLibrary.DBModels.Subscription Subscription { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Subscriptions == null || Subscription == null)
            {
                return PrepareView();
            }

            try
            {
                var userToken = Subscription.AtlassianToken.UserToken;
                Subscription.AtlassianToken = null;

                await subscriptionService.CreatePlan(userToken, Subscription);            
            }
            catch (UtilsLibrary.Exceptions.NotFoundException ex)
            {
                ViewData["tokenMsg"] = ex.Message;
                return PrepareView();
            }
            catch (NotSuitableInputException ex)
            {
                ViewData["errorMsg"] = ex.Message;
                return PrepareView();
            }       
            return RedirectToPage("./Index");
        }
    }
}
