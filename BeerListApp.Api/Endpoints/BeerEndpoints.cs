namespace BeerListApp.Api.Endpoints
{
    public static class BeerEndpoints
    {
        public static void MapBeerEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/beers").WithTags("Beer Endpoints");

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

                return Results.NoContent();
            });
        }
    }
}
