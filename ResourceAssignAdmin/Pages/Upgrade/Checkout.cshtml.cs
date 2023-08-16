using Braintree;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ResourceAssignAdmin.Services;
using System.Numerics;
using UtilsLibrary;
using UtilsLibrary.Exceptions;

namespace ResourceAssignAdmin.Pages.Upgrade
{
    public class CheckoutModel : PageModel
    {
        private readonly IBraintreeService _braintreeService;
        private readonly WoTaasContext _context;
        private readonly ISubscriptionService subscriptionService;

        public CheckoutModel(IBraintreeService braintreeService,
            WoTaasContext context, ISubscriptionService subscriptionService)
        {
            _braintreeService = braintreeService;
            _context = context;
            this.subscriptionService = subscriptionService;
        }

        private const string PaymentErrorMsg = "Unable to make payment, an error has occurred";

        [BindProperty]
        public string PaymentMethod { get; set; } = default!;

        [BindProperty]
        public string UserToken { get; set; } = default!;
        [BindProperty]
        public PlanSubscription Plan { get; set; } = default!;

        public IActionResult PrepareView(string token = "", int? plan = 0)
        {
            PlanSubscription? planFromDB = null;
            planFromDB = _context.PlanSubscriptions.FirstOrDefault(p => p.Id == plan);
            if (planFromDB == null)
                return NotFound();

            if (_context.AtlassianTokens.FirstOrDefault(t => t.UserToken == token)
                == null)
                return NotFound();
            UserToken = token;
       
            var gateway = _braintreeService.GetGateway();
            var clientToken = gateway.ClientToken.Generate();  //Genarate a token
            ViewData["ClientToken"] = clientToken;
            Plan = planFromDB;
            return Page();
        }

        public IActionResult OnGet(string? token, int? plan)
        {
            return PrepareView(token, plan);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Plan.Id = Convert.ToInt32(Request.Form["PlanId"]);
            var user = await _context.AtlassianTokens
                .FirstOrDefaultAsync(a => a.UserToken == UserToken);
            if (user == null)
            {
                ViewData["msg"] = PaymentErrorMsg;
                ViewData["tokenMsg"] = "Invalid Token";
                return PrepareView(UserToken, Plan.Id);
            }

            if (subscriptionService.IsPlusPlan(UserToken))
            {
                ViewData["msg"] = $"{PaymentErrorMsg}: You are already on plus plan";
                return PrepareView(UserToken, Plan.Id);
            }

            var planCurrent = await _context.PlanSubscriptions
                .FirstAsync(ps => ps.Id == Plan.Id);

            IBraintreeGateway gateway;

            gateway = _braintreeService.GetGateway();

            var request = new TransactionRequest
            {
                Amount = Convert.ToDecimal(planCurrent.Price.ToString()),
                PaymentMethodNonce = PaymentMethod,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            Result<Transaction> result = gateway.Transaction.Sale(request);

            if (result.IsSuccess())
            {
                try
                {
                    var newSubscription = new ModelLibrary.DBModels.Subscription()
                    {
                        PlanId = Const.SUBSCRIPTION.PLAN_PLUS,
                        CurrentPeriodStart = DateTime.Now
                    };

                    await subscriptionService.CreatePlan(UserToken, newSubscription);
                }
                catch (UtilsLibrary.Exceptions.NotFoundException ex)
                {
                    ViewData["tokenMsg"] = ex.Message;
                    return PrepareView(UserToken, Plan.Id);
                }
                catch (NotSuitableInputException ex)
                {
                    ViewData["msg"] = ex.Message;
                    return PrepareView(UserToken, Plan.Id);
                }
                return RedirectToPage("PaymentSuccess");
            }
            else
            {
                ViewData["msg"] = $"{PaymentErrorMsg}: {result.Message}";
                return PrepareView(UserToken, Plan.Id);
            }
        }
    }
}
