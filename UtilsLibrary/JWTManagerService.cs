using JiraSchedulingConnectAppService.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UtilsLibrary
{
    public class JWTManagerService
    {
        private readonly IConfiguration? configuration;
        private readonly HttpContext? context;

        public JWTManagerService(HttpContext httpContext)
        {
            this.context = httpContext;
        }

        public JWTManagerService(IConfiguration iconfiguration)
        {
            this.configuration = iconfiguration;
        }
        public string? Authenticate(string accountId, string cloudId)
        {

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim(Const.Claims.ACCOUNT_ID, accountId),
                new Claim(Const.Claims.CLOUD_ID, cloudId)
            };
            var tokenKey = Encoding.UTF8.GetBytes(configuration["JWT:Key"]);
            var securityKey = new SymmetricSecurityKey(tokenKey);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMonths(12),
                signingCredentials: credentials
                );
            var encodeToken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodeToken;
        }

        public string? GetClaim(string claimName)
        {
            string? value = null;
            var identity = context.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                value = identity.FindFirst(claimName)?.Value;
            }
            return value;
        }

        public string? GetCurrentCloudId()
        {
            string? cloudIdRaw = GetClaim(Const.Claims.CLOUD_ID);
            return cloudIdRaw;
        }
    }
}
