using JiraSchedulingConnectAppService.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace JiraSchedulingConnectAppService.Services
{
    public class APIMicroserviceService : IAPIMicroserviceService
    {
        private readonly HttpContext http;
        private readonly HttpClient client;
        private readonly IConfiguration config;

        private string baseUrl = "";

        public APIMicroserviceService(IHttpContextAccessor httpAccessor, IConfiguration config)
        {
            http = httpAccessor.HttpContext;

            client = new HttpClient();
            var bearer = http.Request.Headers["Authorization"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
            var baseUrlList = config.GetSection("Environment:AlgorithmServiceDomains").Get<string[]>();
            baseUrl = baseUrlList[0];
        }


        async Task<HttpResponseMessage> IAPIMicroserviceService.Get(string url)
        {
            var respone = await client.GetAsync(baseUrl + url);
            return respone;
        }

        async Task<HttpResponseMessage> IAPIMicroserviceService.Post(string url, dynamic contentObject)
        {
            //SetBaseURL(cloudId);
            var content = new StringContent(JsonSerializer.Serialize(contentObject));
            var respone = await client.PostAsync(baseUrl + url, content);
            return respone;
            throw new NotImplementedException();
        }
    }
}
