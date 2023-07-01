using ModelLibrary.DTOs.Authentication;
using ModelLibrary.DBModels;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace JiraSchedulingConnectAppService.Services
{
    public class AuthenticationService
    {
        private readonly HttpClient client;
        private readonly JiraDemoContext db;
        private readonly IConfiguration config;

        public AuthenticationService(JiraDemoContext db, IConfiguration config)
        {
            this.client = new HttpClient();
            this.db = db;
            this.config = config;
        }

        async public Task<Object> InitAuthen(string code, string state)
        {
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
                    .FirstOrDefaultAsync(e => e.CloudId == stateContextObject.cloudId && e.AccountInstalledId == stateContextObject.accountId);
                if (tokenFromDB == null)
                {
                    var token = new AtlassianToken()
                    {
                        AccountInstalledId = stateContextObject?.accountId,
                        CloudId = stateContextObject?.cloudId,
                        AccessToken = reponseTokenFirstPhase.access_token,
                        RefressToken = reponseTokenFirstPhase.refresh_token
                    };
                    var tokenInserted = db.AtlassianTokens.Add(token).Entity;
                    db.SaveChanges();

                    var firstAccount = new AccountRole()
                    {
                        AccountId = stateContextObject?.accountId,
                        TokenId = tokenInserted.Id
                    };
                    db.AccountRoles.Add(firstAccount);
                }
                else
                {
                    tokenFromDB.AccessToken = reponseTokenFirstPhase.access_token;
                    tokenFromDB.RefressToken = reponseTokenFirstPhase.refresh_token;
                }
                db.SaveChanges();

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
                response.EnsureSuccessStatusCode();
                return reponseTokenFirstPhase;
            }
            catch (Exception ex)
            {
                db.Database.RollbackTransaction();
                throw;
            }
            finally
            {
                db.Database.CloseConnection();
            }
        }

        async public Task<RepsoneAccessToken?> InitialAcess(string code)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://auth.atlassian.com/oauth/token");
            var domain = config.GetValue<string>("Environment:SelfDomain");
            var exchangeTokenDTO = new ExchangeAccessTokenDTO
            {
                grant_type = "authorization_code",
                client_id = "wDzzxAZSrrM9DtPwZ295BMT3YoFR6KeD",
                client_secret = "ATOAtGENwZ61eAzU1RL-zw2t1e66ITe2XsF-Nj_-SNYuhQZuyDVeA82EytqZf5xUQebU1B2E2744",
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
                client_id = "wDzzxAZSrrM9DtPwZ295BMT3YoFR6KeD",
                client_secret = "ATOAtGENwZ61eAzU1RL-zw2t1e66ITe2XsF-Nj_-SNYuhQZuyDVeA82EytqZf5xUQebU1B2E2744",
                refresh_token = refreshToken,
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

        async public Task<AccessiableResourceResponseDTO[]?> GetUserAccessiableResource(string accessToken)
        {

            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.atlassian.com/oauth/token/accessible-resources");
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var reponseAccessToken = JsonSerializer
                .Deserialize<AccessiableResourceResponseDTO[]>(await response.Content.ReadAsStringAsync());
            return reponseAccessToken;
        }
    }
}
