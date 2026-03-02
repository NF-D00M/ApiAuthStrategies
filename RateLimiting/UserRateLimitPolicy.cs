using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace ApiAuthStrategies.RateLimiting
{
    public class UserRateLimitPolicy : IRateLimiterPolicy<string>
    {
        private readonly IConfiguration _config;

        public UserRateLimitPolicy(IConfiguration config)
        {
            _config = config;
        }

        public RateLimitPartition<string> GetPartition(HttpContext httpContext)
        {
            ClaimsPrincipal user = httpContext.User;
            string username = user.Identity?.Name ?? "anonymous";
            int limit = user.IsInRole("Admin")
                ? int.Parse(_config["RateLimit:Admin"] ?? "0")
                : int.Parse(_config["RateLimit:User"] ?? "0");

            return RateLimitPartition.GetFixedWindowLimiter(username, _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = limit,
                Window = TimeSpan.FromMinutes(1)
            });
        }

        public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected => (context, token) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.HttpContext.Response.WriteAsync("Rate limit exceeded. Try again later.", token);
            return ValueTask.CompletedTask;
        };
    }
}
