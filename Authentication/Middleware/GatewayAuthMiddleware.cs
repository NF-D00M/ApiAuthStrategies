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

            var principal = await processor.ProcessAsync(context);

            if (principal != null) 
            {
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
