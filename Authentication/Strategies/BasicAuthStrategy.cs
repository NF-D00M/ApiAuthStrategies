using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace ApiAuthStrategies.Authentication.Strategies
{
    public class BasicAuthStrategy : IAuthStrategy
    {
        public bool CanHandle(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            return authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<ClaimsPrincipal?> AuthenticateAsync(HttpContext context)
        {
            string authHeaderValue = context.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeaderValue))
            {
                return null;
            }

            try
            {
      
                if (!AuthenticationHeaderValue.TryParse(authHeaderValue, out var authHeader))
                {
                    return null;
                }

                if (authHeader.Scheme != "Basic" || string.IsNullOrEmpty(authHeader.Parameter))
                {
                    return null;
                }
               
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter)).Split(':');

                if (credentials.Length != 2)
                {
                    return null;
                }

                var username = credentials[0];
                var password = credentials[1];

                if (username == "admin" && password == "password123")
                {
                    var claims = new[] {
                        new Claim(ClaimTypes.Name, username),
                        new Claim(ClaimTypes.Role, "Admin")
                    };

                    var identity = new ClaimsIdentity(claims, "Basic");
                    return new ClaimsPrincipal(identity);
                }
            }
            catch (Exception)
            {
                return null; 
            }

            return null;
        }


    }
}
