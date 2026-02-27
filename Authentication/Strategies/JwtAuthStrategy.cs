using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ApiAuthStrategies.Authentication.Strategies
{
    public class JwtAuthStrategy : IAuthStrategy
    {
        private readonly IConfiguration _config;

        public JwtAuthStrategy(IConfiguration config)
        {
            _config = config;
        }

        public bool CanHandle(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();

            return authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                && authHeader.Split('.').Length == 3;
        }

        public async Task<ClaimsPrincipal?> AuthenticateAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)),
                    ValidateIssuer = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _config["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero // Immediate expiration, no grace period
                };

                var principal = handler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                return await Task.FromResult(principal);
            }
            catch
            {
                return null;
            }
        }
               
    }
}
