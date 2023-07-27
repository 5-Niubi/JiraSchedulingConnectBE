using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ResourceAssignAdmin.Common;
using System.Security.Principal;
using UtilsLibrary;

namespace ResourceAssignAdmin.Filter
{
    public class SessionFilter : IPageFilter
    {
        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {

        }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            // Get the Razor Page path
            var pagePath = context.ActionDescriptor.RelativePath;

            // List of URLs that should be excluded from the filter (customize this as needed)
            var excludedUrls = new List<string> {
                "/Pages/Authentication/Login.cshtml",
                "/Pages/Upgrade" };

            // Check if the current page is in the excluded list
            if (excludedUrls.Where(eu => pagePath.Contains(eu)).Count() == 0)
            {
                var currentUser = context.HttpContext.Session.GetString(Const.ADMIN_SERVER.USER);
                if (currentUser == null)
                {
                    context.Result = new RedirectResult("/Authentication/Login");
                }
            }
        }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {

        }
    }
}
