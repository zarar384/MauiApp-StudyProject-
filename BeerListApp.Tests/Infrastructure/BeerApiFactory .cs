using System.Data.Common;
using BeerListApp.Api.Data;
using BeerListApp.Api.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;


namespace BeerListApp.Tests.Infrastructure
{
    public class BeerApiFactory: WebApplicationFactory<Program>
    {
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
            var client = this.CreateClient();

            // Create database tables
            await using var scope = Services.CreateAsyncScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<BeerListAppDbContext>();

            await dbContext.Database.EnsureCreatedAsync();

            // Clear any existing data in the Beers table to ensure a clean state for testing
            await dbContext.Beers.ExecuteDeleteAsync();

            return client;
        }

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
