using System.Security.Claims;

namespace ApiAuthStrategies.Authentication
{
    public interface IAuthStrategy
    {
        bool CanHandle(HttpContext context);

        Task<ClaimsPrincipal?> AuthenticateAsync(HttpContext context);
    }
}
