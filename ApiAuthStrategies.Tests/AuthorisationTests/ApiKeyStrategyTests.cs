using ApiAuthStrategies.Authentication.Strategies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace ApiAuthStrategies.Tests.AuthorisationTests
{
    public class ApiKeyStrategyTests
    {
        [Fact]
        public async Task Authenticate_WithValidApiKey_ReturnsCorrectClaims()
        {
            // Arrange
            Dictionary<string, string> inMemorySettings = new Dictionary<string, string> {
                {"ApiKey:X-API-KEY", "secret-api-key-123"}
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            ApiKeyAuthStrategy strategy = new ApiKeyAuthStrategy(configuration);
            HttpContext context = new DefaultHttpContext();
            context.Request.Headers["X-API-KEY"] = "secret-api-key-123";

            // Act
            ClaimsPrincipal principal = await strategy.AuthenticateAsync(context);

            // Assert
            Assert.NotNull(principal);
            Assert.True(principal.Identity?.IsAuthenticated);
            Assert.Equal("ExternalIntegration", principal.Identity.Name);
            Assert.Contains(principal.Claims, c =>
                c.Type == ClaimTypes.Role &&
                c.Value == "Service"
            );
        }
    }
}