namespace JiraSchedulingConnectAppService.Middlewares
{
    public class AuthenicationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenicationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //Example

            //var cultureQuery = context.Request.Query["culture"];
            //if (!string.IsNullOrWhiteSpace(cultureQuery))
            //{
            //    var culture = new CultureInfo(cultureQuery);

            //    CultureInfo.CurrentCulture = culture;
            //    CultureInfo.CurrentUICulture = culture;
            //}

            // Call the next delegate/middleware in the pipeline.

            await _next(context);

        }
    }

    public static class RequestCultureMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestCulture(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenicationMiddleware>();
        }
    }
}
