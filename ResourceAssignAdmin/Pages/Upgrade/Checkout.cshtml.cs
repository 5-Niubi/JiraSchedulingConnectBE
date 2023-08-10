using Braintree;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ResourceAssignAdmin.Services;
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
        public string PaymentMethod { get; set; } = "";

        [BindProperty]
        public string UserToken { get; set; } = "";

        public IActionResult PrepareView(string? token)
        {
            if (token != null)
            {
                UserToken = token;
            }
            var gateway = _braintreeService.GetGateway();
            var clientToken = gateway.ClientToken.Generate();  //Genarate a token
            ViewData["ClientToken"] = clientToken;

            return Page();
        }

        public IActionResult OnGet(string token)
        {
            return PrepareView(token);
        }

        public async Task<IActionResult> OnPost()
        {
            var user = await _context.AtlassianTokens
                .FirstOrDefaultAsync(a => a.UserToken == UserToken);
            if (user == null)
            {
                ViewData["msg"] = PaymentErrorMsg;
                ViewData["tokenMsg"] = "Invalid Token";
                return PrepareView(null);
            }

            if (subscriptionService.IsPlusPlan(UserToken))
            {
                ViewData["msg"] = $"{PaymentErrorMsg}: You are already on plus plan";
                return PrepareView(null);
            }

            var plan = await _context.PlanSubscriptions
                .FirstAsync(ps => ps.Id == Const.SUBSCRIPTION.PLAN_PLUS);

            IBraintreeGateway gateway;

            gateway = _braintreeService.GetGateway();

            var request = new TransactionRequest
            {
                Amount = Convert.ToDecimal(plan.Price.ToString()),
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
                    return PrepareView(null);
                }
                catch (NotSuitableInputException ex)
                {
                    ViewData["msg"] = ex.Message;
                    return PrepareView(null);
                }
                return RedirectToPage("PaymentSuccess");
            }
            else
            {
                ViewData["msg"] = $"{PaymentErrorMsg}: {result.Message}";
                return PrepareView(null);
            }
        }
    }
}
