
using System.Net;
using System.Net.Http.Json;
using BeerListApp.Api.Models;
using BeerListApp.Tests.Infrastructure;

namespace BeerListApp.Tests.Endpoints
{
    public class LoginEndpointsTests
    {
        // POST /api/login
        [Fact]
        public async Task Login_WhenCredentialsAreValid_ReturnsJwt()
        {
            using var factory = new BeerApiFactory();
            using var client = await factory.CreateTestClientAsync();

            // Create a test user
            await factory.CreateUserAsync("testuser", "TestPassword123!");

            var request = new LoginRequest(
                "testuser",
                "TestPassword123!");

            var response = await client.PostAsJsonAsync("/api/login", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var loginResponse = await response.Content
                .ReadFromJsonAsync<LoginResponse>();

            Assert.NotNull(loginResponse);
            Assert.False(string.IsNullOrWhiteSpace(loginResponse.AccessToken));
            Assert.Equal("Bearer", loginResponse.TokenType);
            Assert.True(loginResponse.ExpiresAtUtc > DateTime.UtcNow);
        }

        // POST /api/login
        [Fact]
        public async Task Login_WhenPasswordIsIncorrect_ReturnsUnauthorized()
        {
            using var factory = new BeerApiFactory();
            using var client = await factory.CreateTestClientAsync();

            await factory.CreateUserAsync("testuser", "TestPassword123!");

            var request = new LoginRequest(
                "testuser",
                "WrongPassword123!");

            var response = await client.PostAsJsonAsync("/api/login", request);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // POST /api/login
        [Fact]
        public async Task Login_WhenUserDoesNotExist_ReturnsUnauthorized()
        {
            using var factory = new BeerApiFactory();
            using var client = await factory.CreateTestClientAsync();

            var request = new LoginRequest(
                "unknownuser",
                "TestPassword123!");

            var response = await client.PostAsJsonAsync("/api/login", request);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // POST /api/login
        [Fact]
        public async Task Login_WhenCredentialsAreEmpty_ReturnsBadRequest()
        {
            using var factory = new BeerApiFactory();
            using var client = await factory.CreateTestClientAsync();

            var request = new LoginRequest("", "");

            var response = await client.PostAsJsonAsync("/api/login", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
