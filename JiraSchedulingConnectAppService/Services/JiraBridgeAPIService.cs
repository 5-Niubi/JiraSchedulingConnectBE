using UtilsLibrary.Exceptions;
using JiraSchedulingConnectAppService.Services.Interfaces;
using ModelLibrary.DBModels;
using System.Net.Http.Headers;
using System.Text.Json;

namespace JiraSchedulingConnectAppService.Services
{
    public class JiraBridgeAPIService : IJiraBridgeAPIService
    {
        private readonly HttpClient client;
        private readonly IAuthenticationService authenticationService;
        private readonly JiraDemoContext db;
        private readonly HttpContext http;

        private string cloudId = "";

        public JiraBridgeAPIService(JiraDemoContext db, IHttpContextAccessor httpAccess
            IAuthenticationService authenticationService)
        {
            this.client = new HttpClient();
            this.authenticationService = authenticationService;
            this.db = db;
            this.http = httpAccess.HttpContext;

            var jwt = new JWTManagerService(http);
            cloudId = jwt.GetCurrentCloudId();
            SetBaseURL();
        }

        private void SetBaseURL()
        {
            this.cloudId = cloudId.ToLower();
            string baseUrl = $"https://api.atlassian.com/ex/jira/{cloudId}";
            client.BaseAddress = new Uri(baseUrl);
        }

        private async System.Threading.Tasks.Task GetNewAccessTokenFromRefreshToken(string? cloudId)
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

        async public Task<HttpResponseMessage> Get(string url)
        {
            await GetNewAccessTokenFromRefreshToken(cloudId);
            var respone = await client.GetAsync(url);
            return respone;
        }

        async public Task<HttpResponseMessage> Post(string url, dynamic contentObject)
        {
            await GetNewAccessTokenFromRefreshToken(cloudId);
            var content = new StringContent(JsonSerializer.Serialize(contentObject));
            var respone = await client.PostAsync(url, content);
            return respone;
        }

        async public Task<HttpResponseMessage> Put(string url, dynamic contentObject)
        {
            await GetNewAccessTokenFromRefreshToken(cloudId);
            var content = new StringContent(JsonSerializer.Serialize(contentObject));
            var respone = await client.PutAsync(url, content);
            return respone;
        }

        async public Task<HttpResponseMessage> Delete(string url)
        {
            await GetNewAccessTokenFromRefreshToken(cloudId);
            var respone = await client.DeleteAsync(url);
            return respone;
        }
    }
}
}
