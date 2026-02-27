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
            var strategy = _strategies.FirstOrDefault(s => s.CanHandle(context));

            if (strategy == null)
            {
                return null;
            }

            return await strategy.AuthenticateAsync(context);
        }
    }
}
