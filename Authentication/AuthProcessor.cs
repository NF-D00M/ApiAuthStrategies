using System.Security.Claims;

namespace ApiAuthStrategies.Authentication
{
    public class AuthProcessor
    {
        private readonly IEnumerable<IAuthStrategy> _strategies;

        public AuthProcessor(IEnumerable<IAuthStrategy> strategies)
        {
            _strategies = strategies;
        }

        public async Task<ClaimsPrincipal?> ProcessAsync(HttpContext context)
        {
            foreach (var strategy in _strategies)
            {
                if (strategy.CanHandle(context)) 
                {
                    return await strategy.AuthenticateAsync(context);
                }
            }
            return null; 
        }
    }
}
