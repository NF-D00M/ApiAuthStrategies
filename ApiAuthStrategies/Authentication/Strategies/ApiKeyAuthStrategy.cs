using System.Security.Claims;

namespace ApiAuthStrategies.Authentication.Strategies
{
    public class ApiKeyAuthStrategy : IAuthStrategy
    {
        private readonly IConfiguration _config;

        public ApiKeyAuthStrategy(IConfiguration config)
        {
            _config = config;
        }

        public bool CanHandle(HttpContext context)
        {
            return context.Request.Headers.ContainsKey("X-API-KEY");   
        }

        public async Task<ClaimsPrincipal?> AuthenticateAsync(HttpContext context)
        {
            string providerKey = context.Request.Headers["X-API-KEY"].ToString();

            if (providerKey == _config["ApiKey:X-API-KEY"])
            {
                Claim[] claims = 
                {
                    new Claim(ClaimTypes.Name, "ExternalIntegration"),
                    new Claim(ClaimTypes.Role, "Service")
                };

                ClaimsIdentity identity = new ClaimsIdentity(claims, "ApiKey");
                return new ClaimsPrincipal(identity);
            }

            return null;
        }      
    }
}
