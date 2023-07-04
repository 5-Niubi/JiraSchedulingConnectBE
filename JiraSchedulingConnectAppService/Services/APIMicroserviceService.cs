﻿using JiraSchedulingConnectAppService.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;

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
            Regex pattern = new Regex(@"Bearer (?<token>[\w.]+)");
            Match match = pattern.Match(bearer);
            string token = match.Groups["token"].Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Set avaiable algorithmServer
            var baseUrlList = config.GetSection("Environment:AlgorithmServiceDomains").Get<string[]>();
            baseUrl = baseUrlList[0];
            client.BaseAddress = new Uri(baseUrl);
        }


        async Task<HttpResponseMessage> IAPIMicroserviceService.Get(string url)
        {     
            var respone = await client.GetAsync(url);

            return respone;
        }

        async Task<HttpResponseMessage> IAPIMicroserviceService.Post(string url, dynamic contentObject)
        {
            var content = new StringContent(JsonSerializer.Serialize(contentObject));
            var respone = await client.PostAsync(url, content);
            return respone;
            throw new NotImplementedException();
        }
    }
}
