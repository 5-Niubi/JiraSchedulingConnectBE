using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UtilsLibrary;

namespace ResourceAssignAdmin.Pages.Authentication
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.Session.Remove(Const.ADMIN_SERVER.USER);
            return RedirectToPage("Login");
        }
    }
}
