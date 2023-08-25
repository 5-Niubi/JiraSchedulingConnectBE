using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using UtilsLibrary;
using UtilsLibrary.Exceptions;

namespace JiraSchedulingConnectAppService.Services
{
    public class APIMicroserviceService : IAPIMicroserviceService
    {
        private readonly HttpContext http;
        private readonly HttpClient client;
        private readonly IConfiguration config;

        private string? baseUrl = null;

        public APIMicroserviceService(IHttpContextAccessor httpAccessor, IConfiguration config)
        {
            http = httpAccessor.HttpContext;
            client = new HttpClient();
            client.Timeout = Timeout.InfiniteTimeSpan;

            var bearer = http.Request.Headers["Authorization"];
            bearer = bearer.IsNullOrEmpty() ? "Bearer " : bearer;
            Regex pattern = new(@"Bearer (?<token>[\w.]+)");
            Match match = pattern.Match(bearer);
            string token = match.Groups["token"].Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            this.config = config;
        }

        public void SetDomain(string configObject)
        {
            // Set avaiable algorithmServer
            var baseUrl = config.GetSection(configObject).Get<string>();
            client.BaseAddress = new Uri(baseUrl);
        }

        async Task<HttpResponseMessage> IAPIMicroserviceService.Get(string url)
        {
            int count = Const.RETRY_API_TIME;
            bool retry = false;
            HttpResponseMessage? respone = null;
            do
            {
                try
                {
                    respone = await client.GetAsync(url);
                }
                catch (HttpRequestException)
                {
                    retry = true;
                    if (count-- == 0)
                    {
                        break;
                    }
                }
            } while (retry);

            if (!(respone?.IsSuccessStatusCode ?? false))
            {
                throw new MicroServiceAPIException(
                    await respone.Content.ReadAsStringAsync(),
                    Const.MESSAGE.MICROSERVICE_API_ERROR,
                    respone?.StatusCode);
            }
            return respone;
        }

        async Task<HttpResponseMessage> IAPIMicroserviceService.Post(string url, dynamic contentObject)
        {
            var content = new StringContent(JsonSerializer.Serialize(contentObject));
            var respone = await client.PostAsync(url, content);
            if (!respone.IsSuccessStatusCode)
            {
                throw new MicroServiceAPIException(await respone.Content.ReadAsStringAsync(),
                    Const.MESSAGE.MICROSERVICE_API_ERROR);
            }
            return respone;
        }
    }
}
