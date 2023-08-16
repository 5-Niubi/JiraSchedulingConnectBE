using JiraSchedulingConnectAppService.Services.Interfaces;
using System.Globalization;
using System.Net;

namespace JiraSchedulingConnectAppService.MiddleWare
{
    public class GetRequestIPMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerManager _Logger;


        public GetRequestIPMiddleWare(RequestDelegate next,
            ILoggerManager logger)
        {
            _next = next;
            _Logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            IPAddress? remoteIpAddress = context.Connection.RemoteIpAddress;
            string result = "";
            if (remoteIpAddress != null)
            {
                // If we got an IPV6 address, then we need to ask the network for the IPV4 address 
                // This usually only happens when the browser is on the same machine as the server.
                if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    remoteIpAddress = System.Net.Dns.GetHostEntry(remoteIpAddress).AddressList
            .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                }
                result = remoteIpAddress.ToString();
            }
            _Logger.LogInfo($"Request IP: {result}");

            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }
    }

    public static class RequestCultureMiddlewareExtensions
    {
        public static IApplicationBuilder UseGetRequestIPMiddleWare(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GetRequestIPMiddleWare>();
        }
    }
}
