using ModelLibrary.DTOs.Authentication;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<Object> InitAuthen(string? code, string? state, string? error, string? error_description);
        public Task<RepsoneAccessToken> ExchangeAccessAndRefreshToken(string refreshToken);
        public TokenForDownloadDTO GetTokenForHandshakeDownload();
    }
}
