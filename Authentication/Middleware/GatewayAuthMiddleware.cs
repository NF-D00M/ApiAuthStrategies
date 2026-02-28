using System.Security.Claims;

namespace ApiAuthStrategies.Authentication.Middleware
{
    public class GatewayAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public GatewayAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AuthProcessor processor)
        {
            if (context.Request.Path.StartsWithSegments("/api/auth/login"))
            {
                await _next(context);
                return;
            }

            ClaimsPrincipal principal = await processor.ProcessAsync(context);

            if (principal != null) 
            {
                string methodUsed = principal.Identity?.AuthenticationType;
                Console.WriteLine($"User authenticated via: {methodUsed}");

                context.User = principal;
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid or Missing Credentials");
            }
        }
    }
}
