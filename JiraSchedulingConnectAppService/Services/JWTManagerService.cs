using UtilsLibrary;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JiraSchedulingConnectAppService.Services
{
    public class JWTManagerService
    {
        private readonly IConfiguration? configuration;
        private readonly HttpContext? context;

        public JWTManagerService(HttpContext? httpContext)
        {
            this.context = httpContext;
        }

        public JWTManagerService(IConfiguration iconfiguration)
        {
            this.configuration = iconfiguration;
        }

        public JWTManagerService(HttpContext? httpContext, IConfiguration iconfiguration)
        {
            this.context = httpContext;
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

        public string GenerateToken(string cloudId)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim(Const.Claims.CLOUD_ID, cloudId)
            };
            var tokenKey = Encoding.UTF8.GetBytes(configuration["JWT:Key"]);
            var securityKey = new SymmetricSecurityKey(tokenKey);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials
                );
            var encodeToken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodeToken;
        }

        public bool ValidateJwt(string token)
        {
            string secretKey = configuration["JWT:Key"];

            // Define the validation parameters
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,              // Validate the token issuer
                ValidateAudience = true,            // Validate the token audience
                ValidateLifetime = true,            // Validate the token expiration
                ValidateIssuerSigningKey = true,     // Validate the token signature
                ValidIssuer = configuration["Jwt:Issuer"],         // Set the valid issuer
                ValidAudience = configuration["Jwt:Audience"],     // Set the valid audience
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)) // Set the secret key
            };

            try
            {
                // Validate the token
                ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(token,
                    validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch (Exception ex)
            {
                // Token validation failed
                return false;
            }
        }

        public string GetClaim(string claimName)
        {
            string value = "";
            var identity = context?.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                value = identity.FindFirst(claimName) == null ? "" : identity.FindFirst(claimName).Value;
            }
            return value;
        }

        public string GetCurrentCloudId()
        {
            string cloudIdRaw = GetClaim(Const.Claims.CLOUD_ID);
            return cloudIdRaw;
        }
    }
}
