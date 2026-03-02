using ApiAuthStrategies.Authentication.Strategies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiAuthStrategies.Tests.AuthorisationTests
{
    public class JwtStrategyTests
    {
        [Fact]
        public async Task Authenticate_WithValidJwt_ReturnsCorrectClaims()
        {
            // Arrange

            Dictionary<string, string> inMemorySettings = new Dictionary<string, string> {
                {"Jwt:Key", "00000000000000000000000000000000"},
                {"Jwt:Issuer", "https://your-auth-server.com"},
                {"Jwt:Audience", "https://your-api-gateway.com"}
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            Claim[] claims =
            {
                new Claim(ClaimTypes.Name, "jwtuser"),
                new Claim(ClaimTypes.Role, "JwtRole")
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: creds
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            HttpContext context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = $"Bearer {tokenString}";

            JwtAuthStrategy strategy = new JwtAuthStrategy(configuration);

            // Act

            ClaimsPrincipal principal = await strategy.AuthenticateAsync(context);

            // Assert

            Assert.NotNull(principal);
            Assert.True(principal.Identity?.IsAuthenticated);
            Assert.Equal("jwtuser", principal.Identity.Name);
            Assert.Contains(principal.Claims, c =>
                c.Type == ClaimTypes.Role &&
                c.Value == "JwtRole"
            );
        }
    }
}