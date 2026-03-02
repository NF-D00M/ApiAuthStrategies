using ApiAuthStrategies.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace ApiAuthStrategies.Tests.ControllerTests
{
    public class AuthControllerTests
    {
        [Fact]
        public void Login_WithValidCredentials_ReturnsJwtToken()
        {
            // Arrange

            Dictionary<string, string> inMemorySettings = new Dictionary<string, string> {
                {"Jwt:Key", "00000000000000000000000000000000"},
                {"Jwt:Issuer", "https://your-auth-server.com"},
                {"Jwt:Audience", "TestAudihttps://your-api-gateway.comence"},
                {"Jwt:ExpiryMinutes", "5"}
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            AuthController controller = new AuthController(configuration);
            LoginRequest request = new LoginRequest
            {
                Username = "admin",
                Password = "password123"
            };

            // Act

            OkObjectResult result = controller.Login(request) as OkObjectResult;

            // Assert

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Value);
            PropertyInfo tokenProperty = result.Value.GetType().GetProperty("token");
            Assert.NotNull(tokenProperty);
            string token = tokenProperty.GetValue(result.Value) as string;
            Assert.False(string.IsNullOrEmpty(token));
        }
    }
}