using ApiAuthStrategies.Authentication;
using ApiAuthStrategies.Authentication.Middleware;
using ApiAuthStrategies.Authentication.Strategies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddScoped<IAuthStrategy, BasicAuthStrategy>();
builder.Services.AddScoped<IAuthStrategy, ApiKeyAuthStrategy>();
builder.Services.AddScoped<IAuthStrategy, JwtAuthStrategy>();
builder.Services.AddScoped<AuthProcessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseMiddleware<GatewayAuthMiddleware>();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

