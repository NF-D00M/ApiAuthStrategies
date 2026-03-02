using ApiAuthStrategies.Authentication.Strategies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text;

namespace ApiAuthStrategies.Tests.AuthorisationTests
{
    public class BasicAuthStrategyTests
    {
        [Fact]
        public async Task Authenticate_WithValidCredentials_ReturnsCorrectClaims()
        {
            // Arrange

            HttpContext context = new DefaultHttpContext();
            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("admin:password123"));
            string expectedHeader = $"Basic {credentials}";
            context.Request.Headers["Authorization"] = $"Basic {credentials}";

            // Act
            BasicAuthStrategy strategy = new BasicAuthStrategy();
            ClaimsPrincipal principal = await strategy.AuthenticateAsync(context);

            // Assert

            Assert.Equal(expectedHeader, context.Request.Headers["Authorization"]);

            Assert.NotNull(principal);
            Assert.True(principal.Identity?.IsAuthenticated);
            Assert.Equal("admin", principal.Identity.Name);
            Assert.Contains(principal.Claims, c => 
                c.Type == ClaimTypes.Role && 
                c.Value == "Admin"
            );
        }
    }
}
