using System.Net;
using System.Net.Http.Json;
using BeerListApp.Api.Entities;
using BeerListApp.Tests.Infrastructure;

namespace BeerListApp.Tests.Endpoints
{
    public class BeerEndpointsTests
    {
        // GET /api/beers
        [Fact]
        public async Task GetAll_WhenDatabaseIsEmpty_ReturnsEmptyList()
        {
            using var factory = new BeerApiFactory();
            using var client = await factory.CreateTestClientAsync();

            var response = await client.GetAsync("/api/beers");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var beers = await response.Content
                .ReadFromJsonAsync<List<Beer>>();

            Assert.NotNull(beers);
            Assert.Empty(beers);
        }

        // GET /api/beers
        [Fact]
        public async Task GetAll_WhenBeersExist_ReturnsAllBeers()
        {
            using var factory = new BeerApiFactory();
            using var client = await factory.CreateTestClientAsync();

            var firstId = await factory.AddBeerAsync("Pilsner", "Czech lager", 5);
            var secondId = await factory.AddBeerAsync("Stout", "Dark beer", 6);

            var response = await client.GetAsync("/api/beers");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var beers = await response.Content
                .ReadFromJsonAsync<List<Beer>>();

            Assert.NotNull(beers);
            Assert.Equal(2, beers.Count);

            Assert.Contains(beers, beer => beer.Id == firstId);
            Assert.Contains(beers, beer => beer.Id == secondId);
        }

        // GET /api/beers/{id}
        [Fact]
        public async Task GetById_WhenBeerExists_ReturnsBeer()
        {
            using var factory = new BeerApiFactory();
            using var client = await factory.CreateTestClientAsync();

            var beerId = await factory.AddBeerAsync(
                "Pilsner", "Czech lager", 5);

            var response = await client.GetAsync(
                $"/api/beers/{beerId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var beer = await response.Content
                .ReadFromJsonAsync<Beer>();

            Assert.NotNull(beer);
            Assert.Equal(beerId, beer.Id);
            Assert.Equal("Pilsner", beer.Name);
            Assert.Equal("Czech lager", beer.Description);
            Assert.Equal(5d, Convert.ToDouble(beer.AlcoholContent));
        }

        // GET /api/beers/{id}
        [Fact]
        public async Task GetById_WhenBeerDoesNotExist_ReturnsNotFound()
        {
            using var factory = new BeerApiFactory();
            using var client = await factory.CreateTestClientAsync();

            var response = await client.GetAsync("/api/beers/999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // POST /api/beers
        [Fact]
        public async Task Create_WhenBeerIsValid_ReturnsCreated()
        {
            using var factory = new BeerApiFactory();
            using var client = await factory.CreateTestClientAsync();

            var newBeer = new
            {
                Name = "Guinness",
                Description = "Irish stout",
                AlcoholContent = 4
            };

            var response = await client.PostAsJsonAsync("/api/beers", newBeer);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var createdBeer = await response.Content
                .ReadFromJsonAsync<Beer>();

            Assert.NotNull(createdBeer);
            Assert.True(createdBeer.Id > 0);
            Assert.Equal("Guinness", createdBeer.Name);

            Assert.Equal($"/api/beers/{createdBeer.Id}", response.Headers.Location?.ToString());

            // Verify that the beer was saved in the database.
            var savedBeer = await factory.FindBeerAsync(createdBeer.Id);

            Assert.NotNull(savedBeer);
            Assert.Equal("Guinness", savedBeer.Name);
            Assert.Equal("Irish stout", savedBeer.Description);
            Assert.Equal(4d, Convert.ToDouble(savedBeer.AlcoholContent));
        }

        // PUT /api/beers/{id}
        [Fact]
        public async Task Update_WhenBeerExists_ReturnsNoContent()
        {
            using var factory = new BeerApiFactory();
            using var client = await factory.CreateTestClientAsync();

            var beerId = await factory.AddBeerAsync("Old Beer", "Old description", 4);

            var updatedBeer = new
            {
                Name = "Updated Beer",
                Description = "Updated description",
                AlcoholContent = 7
            };

            var response = await client.PutAsJsonAsync($"/api/beers/{beerId}", updatedBeer);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify changes in the database.
            var savedBeer = await factory.FindBeerAsync(beerId);

            Assert.NotNull(savedBeer);
            Assert.Equal(beerId, savedBeer.Id);
            Assert.Equal("Updated Beer", savedBeer.Name);
            Assert.Equal("Updated description", savedBeer.Description);
            Assert.Equal(7d, Convert.ToDouble(savedBeer.AlcoholContent));
        }

        // PUT /api/beers/{id}
        [Fact]
        public async Task Update_WhenBeerDoesNotExist_ReturnsNotFound()
        {
            using var factory = new BeerApiFactory();
            using var client = await factory.CreateTestClientAsync();

            var updatedBeer = new
            {
                Name = "Unknown Beer",
                Description = "Not found",
                AlcoholContent = 5
            };

            var response = await client.PutAsJsonAsync($"/api/beers/999", updatedBeer);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            // PUT must not create a missing beer in this API.
            var beer = await factory.FindBeerAsync(999);

            Assert.Null(beer);
        }

        // DELETE /api/beers/{id}
        [Fact]
        public async Task Delete_WhenBeerExists_ReturnsNoContent()
        {
            using var factory = new BeerApiFactory();
            using var client = await factory.CreateTestClientAsync();

            var beerId = await factory.AddBeerAsync("Beer to delete", "Temporary beer", 5);

            var response = await client.DeleteAsync($"/api/beers/{beerId}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify the beer no longer exists.
            var deletedBeer = await factory.FindBeerAsync(beerId);

            Assert.Null(deletedBeer);
        }

        // DELETE /api/beers/{id}
        [Fact]
        public async Task Delete_WhenBeerDoesNotExist_ReturnsNotFound()
        {
            using var factory = new BeerApiFactory();
            using var client = await factory.CreateTestClientAsync();

            var response = await client.DeleteAsync("/api/beers/999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}