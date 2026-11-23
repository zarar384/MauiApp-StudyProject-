
using System.Data.Common;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using BeerListApp.Api;
using BeerListApp.Api.Data;
using BeerListApp.Api.Entities;
using BeerListApp.Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace BeerListApp.Tests.Infrastructure
{
    public class BeerApiFactory : WebApplicationFactory<Program>
    {
        private const string TestUsername = "testuser";
        private const string TestPassword = "TestPassword123!";

        private static readonly string TestJwtKey =
            Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        protected override IHost CreateHost(IHostBuilder builder)
        {
            // Configure JWT settings before Program.cs initializes services
            builder.ConfigureHostConfiguration(configuration =>
            {
                configuration.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["Jwt:Issuer"] = "BeerListApp.Tests",
                        ["Jwt:Audience"] = "BeerListApp.Tests.Client",
                        ["Jwt:Key"] = TestJwtKey,
                        ["Jwt:AccessTokenMinutes"] = "30",
                        ["Cors:AllowedOrigin"] = "http://localhost:4200"
                    });
            });

            return base.CreateHost(builder);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Remove the app's DbContext configuration
                services.RemoveAll<DbContextOptions<BeerListAppDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<BeerListAppDbContext>>();

                services.RemoveAll<DbConnection>();

                // Create an isolated in-memory SQLite database for testing
                services.AddSingleton<DbConnection>(_ =>
                {
                    var connection = new SqliteConnection("DataSource=:memory:");
                    connection.Open();

                    return connection;
                });

                // Register the DbContext with the test database connection
                services.AddDbContext<BeerListAppDbContext>((serviceProvider, options) =>
                {
                    var connection = serviceProvider.GetRequiredService<DbConnection>();

                    options.UseSqlite(connection);
                });
            });
        }

        public async Task<HttpClient> CreateTestClientAsync()
        {
            // Use HTTPS to avoid redirection in the Testing environment
            var client = CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost")
            });

            // Create database tables
            await using var scope = Services.CreateAsyncScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<BeerListAppDbContext>();

            await dbContext.Database.EnsureCreatedAsync();

            // Clear any existing data in the Beers table to ensure a clean state for testing
            await dbContext.Beers.ExecuteDeleteAsync();

            return client;
        }

        public async Task<HttpClient> CreateAuthorizedClientAsync()
        {
            // Create a test client and initialize the database
            var client = await CreateTestClientAsync();

            // Create a test user with ASP.NET Core Identity
            await CreateUserAsync(TestUsername, TestPassword);

            // Authenticate the test user through the login endpoint
            var response = await client.PostAsJsonAsync(
                "/api/login",
                new LoginRequest(TestUsername, TestPassword));

            response.EnsureSuccessStatusCode();

            // Read the JWT access token from the login response
            var loginResponse = await response.Content
                .ReadFromJsonAsync<LoginResponse>()
                ?? throw new InvalidOperationException(
                    "Login response is empty.");

            // Add JWT to all subsequent HTTP requests
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    loginResponse.AccessToken);

            return client;
        }

        #region Helper methods for authentication

        /// <summary>
        /// Create a test user using ASP.NET Core Identity.
        /// </summary>
        public async Task CreateUserAsync(
            string username,
            string password)
        {
            await using var scope = Services.CreateAsyncScope();

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<IdentityUser>>();

            var user = new IdentityUser
            {
                UserName = username
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"Failed to create test user: {errors}");
            }
        }

        #endregion

        #region Helper methods to add, find beers

        /// <summary>
        /// Add a beer to the test database and return its ID.
        /// </summary>
        /// <returns>The ID of the newly added beer.</returns>
        public async Task<int> AddBeerAsync(
            string name,
            string description,
            int alcoholContent)
        {
            await using var scope = Services.CreateAsyncScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<BeerListAppDbContext>();

            var beer = new Beer
            {
                Name = name,
                Description = description,
                AlcoholContent = alcoholContent
            };

            dbContext.Beers.Add(beer);
            await dbContext.SaveChangesAsync();

            return beer.Id;
        }

        /// <summary>
        /// Find the beer with the specified ID in the test database.
        /// </summary>
        /// <param name="id">The ID of the beer to find.</param>
        /// <returns>The beer with the specified ID, or null if not found.</returns>
        public async Task<Beer?> FindBeerAsync(int id)
        {
            await using var scope = Services.CreateAsyncScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<BeerListAppDbContext>();

            return await dbContext.Beers
                .AsNoTracking()
                .FirstOrDefaultAsync(beer => beer.Id == id);
        }

        #endregion
    }
}
