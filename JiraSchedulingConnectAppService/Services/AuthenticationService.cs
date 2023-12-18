using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Authentication;
using System.Text;
using System.Text.Json;
using UtilsLibrary;
using UtilsLibrary.Exceptions;

namespace JiraSchedulingConnectAppService.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient client;
        private readonly WoTaasContext db;
        private readonly IConfiguration config;
        private readonly HttpContext? http;

        public AuthenticationService(WoTaasContext db, IConfiguration config,
            IHttpContextAccessor httpAcc)
        {
            client = new HttpClient();
            this.db = db;
            this.config = config;
            http = httpAcc.HttpContext;
        }

        async public Task<Object> InitAuthen(string code, string state, string? error, string? error_description)
        {

            if (error != null || error_description != null)
            {
                throw new UnAuthorizedException(error_description);
            }

            // Handle State
            db.Database.BeginTransaction();
            try
            {
                string stateStr = Encoding.UTF8.GetString(Convert.FromBase64String(state));
                var stateContextObject = JsonSerializer.Deserialize<StateContextObjectCallback>(stateStr);

                var jwtManager = new JWTManagerService(config);
                // *Token
                var jwtToken = jwtManager.Authenticate(stateContextObject.accountId, stateContextObject.cloudId);

                var reponseTokenFirstPhase = await InitialAcess(code);
                var accessiableResourceResponseDTO = await GetUserAccessiableResource(reponseTokenFirstPhase.access_token);

                var tokenFromDB = await db.AtlassianTokens
                    .FirstOrDefaultAsync(e => e.CloudId == stateContextObject.cloudId);
                if (tokenFromDB == null)
                {
                    // Create new
                    var token = new AtlassianToken()
                    {
                        AccountInstalledId = stateContextObject?.accountId,
                        CloudId = stateContextObject?.cloudId,
                        AccessToken = reponseTokenFirstPhase.access_token,
                        RefressToken = reponseTokenFirstPhase.refresh_token,
                        Site = stateContextObject?.siteUrl,
                        UserToken = Utils.RandomString(15)
                    };

                    var subscription = new Subscription()
                    {
                        PlanId = 1,
                        CurrentPeriodStart = DateTime.Now,
                        Token = Utils.RandomString(10)
                    };

                    token.Subscriptions.Add(subscription);

                    var firstAccount = new AccountRole()
                    {
                        AccountId = stateContextObject?.accountId,
                    };

                    token.AccountRoles.Add(firstAccount);

                    var tokenInserted = db.AtlassianTokens.Add(token).Entity;
                }
                else
                {
                    tokenFromDB.AccountInstalledId = stateContextObject?.accountId;
                    // Update existed
                    tokenFromDB.AccessToken = reponseTokenFirstPhase.access_token;
                    tokenFromDB.RefressToken = reponseTokenFirstPhase.refresh_token;
                }
                db.SaveChanges();
                await db.Database.CommitTransactionAsync();
                // Hand saking with forge client
                var request = new HttpRequestMessage(HttpMethod.Post, stateContextObject.triggerUrl);
                var webTriggerRequestDTO = new WebTriggerCallbackBodyDTO.Request
                {
                    token = jwtToken
                };
                var jsonWebTriggerRequestDTO = JsonSerializer.Serialize(webTriggerRequestDTO);
                var content = new StringContent(jsonWebTriggerRequestDTO, null, "application/json");

                request.Content = content;
                var response = await client.SendAsync(request);
                return reponseTokenFirstPhase;
            }

            catch (Exception)
            {
                db.Database.RollbackTransaction();
                throw;
            }
            finally
            {
                db.Database.CloseConnection();
            }
        }

        async private Task<RepsoneAccessToken?> InitialAcess(string code)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://auth.atlassian.com/oauth/token");
            var domain = config.GetValue<string>("Environment:SelfDomain");
            //var domain = Utils.GetSelfDomain(http);
            var exchangeTokenDTO = new ExchangeAccessTokenDTO
            {
                grant_type = "authorization_code",
                client_id = config["ConnectAppKey:CliendId"],
                client_secret = config["ConnectAppKey:ClientSecret"],
                code = code,
                redirect_uri = $"{domain}/Authentication/Callback",
            };
            var jsonExchangeDTO = JsonSerializer.Serialize(exchangeTokenDTO);
            var content = new StringContent(jsonExchangeDTO, null, "application/json");

            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var reponseAccessToken = JsonSerializer
                .Deserialize<RepsoneAccessToken>(await response.Content.ReadAsStringAsync());
            return reponseAccessToken;
        }

        async public Task<RepsoneAccessToken> ExchangeAccessAndRefreshToken(string refreshToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://auth.atlassian.com/oauth/token");
            var exchangeTokenDTO = new RequestExchangeAccessAndRefeshTokenDTO
            {
                grant_type = "refresh_token",
                client_id = config["ConnectAppKey:CliendId"],
                client_secret = config["ConnectAppKey:ClientSecret"],
                refresh_token = refreshToken,
            };
            var jsonExchangeDTO = JsonSerializer.Serialize(exchangeTokenDTO);

            var content = new StringContent(jsonExchangeDTO, null, "application/json");

            request.Content = content;
            var response = await client.SendAsync(request);
            var reponseAccessToken = JsonSerializer
                .Deserialize<RepsoneAccessToken>(await response.Content.ReadAsStringAsync());
            return reponseAccessToken;
        }

        async private Task<AccessiableResourceResponseDTO[]?> GetUserAccessiableResource(string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.atlassian.com/oauth/token/accessible-resources");
            request.Headers.Add("Authorization", $"Bearer {accessToken}");

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var reponseAccessToken = JsonSerializer
                .Deserialize<AccessiableResourceResponseDTO[]>(await response.Content.ReadAsStringAsync());
            return reponseAccessToken;
        }

        public TokenForDownloadDTO GetTokenForHandshakeDownload()
        {
            var tokenManager = new JWTManagerService(http, config);

            var cloudId = tokenManager.GetCurrentCloudId();

            var token = tokenManager.GenerateToken(cloudId);
            var tokenDTO = new TokenForDownloadDTO()
            {
                token = token
            };
            return tokenDTO;
        }
    }
}