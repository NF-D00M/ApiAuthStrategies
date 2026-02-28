using ApiAuthStrategies.Authentication;
using ApiAuthStrategies.Authentication.Middleware;
using ApiAuthStrategies.Authentication.Strategies;
using ApiAuthStrategies.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Rate limiter
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy<string, UserRateLimitPolicy>("UserBoundPolicy");
});

// Controllers
builder.Services.AddControllers();

// Authentication
builder.Services.AddScoped<IAuthStrategy, BasicAuthStrategy>();
builder.Services.AddScoped<IAuthStrategy, ApiKeyAuthStrategy>();
builder.Services.AddScoped<IAuthStrategy, JwtAuthStrategy>();
builder.Services.AddScoped<AuthProcessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<GatewayAuthMiddleware>();
app.UseRateLimiter();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();

