
namespace BeerListApp.Api.Endpoints
{
    public static class BeerEndpoints
    {
        public static void MapBeerEndpoints(this IEndpointRouteBuilder app)
        {
            var logger = app.ServiceProvider
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger(typeof(BeerEndpoints).FullName!);

            var group = app.MapGroup("api/beers")
                .WithTags("Beer Endpoints");

            // GET: api/beers
            // Returns a list of all beers in the database.
            group.MapGet("/", async (BeerListAppDbContext dbContext) =>
            {
                var beers = await dbContext.Beers.ToListAsync();

                return Results.Ok(beers);
            });

            // GET: api/beers/{id}
            // Returns a specific beer by its ID.
            group.MapGet("/{id:int}", async (int id, BeerListAppDbContext dbContext) =>
            {
                var beer = await dbContext.Beers.FindAsync(id);

                if (beer == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(beer);
            });

            // POST: api/beers
            // Creates a new beer in the database.
            group.MapPost("/", async (Beer beer, BeerListAppDbContext dbContext) =>
            {
                dbContext.Beers.Add(beer);
                await dbContext.SaveChangesAsync();

                logger.LogInformation("Beer created successfully. BeerId: {BeerId}", beer.Id);

                return Results.Created($"/api/beers/{beer.Id}", beer);
            });

            // PUT: api/beers/{id}
            // Updates an existing beer in the database.
            group.MapPut("/{id:int}", async (int id, Beer updatedBeer, BeerListAppDbContext dbContext) =>
            {
                var beer = await dbContext.Beers.FindAsync(id);

                if (beer == null)
                {
                    return Results.NotFound();
                }

                beer.Name = updatedBeer.Name;
                beer.Description = updatedBeer.Description;
                beer.AlcoholContent = updatedBeer.AlcoholContent;

                await dbContext.SaveChangesAsync();

                logger.LogInformation("Beer updated successfully. BeerId: {BeerId}", id);

                return Results.NoContent();
            });

            // DELETE: api/beers/{id}
            // Deletes a specific beer from the database.
            group.MapDelete("/{id:int}", async (int id, BeerListAppDbContext dbContext) =>
            {
                var beer = await dbContext.Beers.FindAsync(id);

                if (beer == null)
                {
                    return Results.NotFound();
                }

                dbContext.Beers.Remove(beer);
                await dbContext.SaveChangesAsync();

                logger.LogInformation("Beer deleted successfully. BeerId: {BeerId}", id);

                return Results.NoContent();
            });

            // GET: api/beers/health
            group.MapGet("/health", async (BeerListAppDbContext dbContext, CancellationToken cancellationToken) =>
            {
                var isConnected = await dbContext.Database
                    .CanConnectAsync(cancellationToken);

                if (!isConnected)
                {
                    logger.LogWarning("Database health check failed. Database connection unavailable.");

                    return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
                }

                return Results.Ok();
            });
        }
    }
}
