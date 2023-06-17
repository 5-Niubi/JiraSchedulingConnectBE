using JiraSchedulingConnectAppService.Exceptions;
using JiraSchedulingConnectAppService.Models;
using JiraSchedulingConnectAppService.Services;
using System.Net.Http.Headers;
using System.Text.Json;

namespace JiraSchedulingConnectAppService.Common
{
    public class APICommon
    {
        private readonly HttpClient client;
        private readonly AuthenticationService authenticationService;
        private readonly JiraDemoContext db;

        private string cloudId = "";
        private string baseUrl = "";


        public APICommon(JiraDemoContext db, IConfiguration config)
        {
            this.client = new HttpClient();
            this.authenticationService = new AuthenticationService(db, config);
            this.db = db;

        }

        private string SetBaseURL(string cloudId)
        {
            this.cloudId = cloudId.ToLower();
            this.baseUrl = $"https://api.atlassian.com/ex/jira/{cloudId}";
            return baseUrl;
        }

        private async System.Threading.Tasks.Task GetNewAccessTokenFromRefreshToken(string cloudId)
        {

            var tokenFromDB = db.AtlassianTokens.FirstOrDefault(e => e.CloudId == cloudId);
            if (tokenFromDB == null)
            {
                //TODO: Handle token null from db;
                throw new UnAuthorizedException("Atlassian Token Not Found From DB");
            }
            else
            {
                var responseToken = await authenticationService.ExchangeAccessAndRefreshToken(tokenFromDB.RefressToken);
                tokenFromDB.AccessToken = responseToken.access_token;
                tokenFromDB.RefressToken = responseToken.refresh_token;

                db.AtlassianTokens.Update(tokenFromDB);
                // Add AccessToken to bearer header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", responseToken.access_token);

            }
            await db.SaveChangesAsync();
        }

        public async Task<HttpResponseMessage> Get(string url, HttpContext context)
        {
            string? cloudId = JWTManagerService.GetCurrentCloudId(context);
            await GetNewAccessTokenFromRefreshToken(cloudId);
            SetBaseURL(cloudId);
            var respone = await client.GetAsync(baseUrl + url);
            return respone;
        }

        public async Task<HttpResponseMessage> Post(string url, dynamic contentObject, HttpContext context)
        {
            string? cloudId = JWTManagerService.GetCurrentCloudId(context);
            await GetNewAccessTokenFromRefreshToken(cloudId);
            SetBaseURL(cloudId);
            var content = new StringContent(JsonSerializer.Serialize(contentObject));
            var respone = await client.PostAsync(baseUrl + url, content);
            return respone;
        }

        public async Task<HttpResponseMessage> Put(string url, dynamic contentObject, HttpContext context)
        {
            string? cloudId = JWTManagerService.GetCurrentCloudId(context);
            await GetNewAccessTokenFromRefreshToken(cloudId);
            SetBaseURL(cloudId);
            var content = new StringContent(JsonSerializer.Serialize(contentObject));
            var respone = await client.PutAsync(baseUrl + url, content);
            return respone;
        }

        public async Task<HttpResponseMessage> Delete(string url, HttpContext context)
        {
            string? cloudId = JWTManagerService.GetCurrentCloudId(context);
            await GetNewAccessTokenFromRefreshToken(cloudId);
            SetBaseURL(cloudId);
            var respone = await client.DeleteAsync(baseUrl + url);
            return respone;
        }
    }
}