using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using Newtonsoft.Json;

namespace ResourceAssignAdmin.Pages.Authentication
{
    public class LoginModel : PageModel
    {
        private readonly JiraDemoContext db;

        public LoginModel(JiraDemoContext db)
        {
            this.db = db;
        }

        [BindProperty]
        public AdminAccount AdminAccount { get; set; }

        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            var accountLogin = db.AdminAccounts.Where(a => a.Username == AdminAccount.Username
              && a.Password == AdminAccount.Password).FirstOrDefault();
            if (accountLogin == null)
            {
                ViewData["errorMsg"] = "Username or Password is invalid";
                return Page();
            }
            var accountSession = new AdminAccount()
            {
                Id = accountLogin.Id,
                Username = accountLogin.Username,
                Avatar = accountLogin.Avatar,
                Email = accountLogin.Email,
                CreateDatetime = accountLogin.CreateDatetime,
            };
            HttpContext.Session.SetString("user", JsonConvert.SerializeObject(accountSession));

            return RedirectToPage("/Index");
        }
    }
}
