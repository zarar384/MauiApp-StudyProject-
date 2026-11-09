namespace BeerListApp.Api.Data
{
    public class BeerListAppDbContext : DbContext
    {
        public DbSet<Beer> Beers { get; set; }

        public BeerListAppDbContext(DbContextOptions<BeerListAppDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Beer>()
                // Seed initial data for the Beers table
                .HasData(
                    new Beer { Id = 1, Name = "Pale Ale", Description = "A light and refreshing ale with a balanced hop flavor.", AlcoholContent = 5.0 },
                    new Beer { Id = 2, Name = "Stout", Description = "A dark and rich beer with roasted malt flavors.", AlcoholContent = 6.5 },
                    new Beer { Id = 3, Name = "IPA", Description = "An India Pale Ale with a strong hop aroma and bitterness.", AlcoholContent = 7.0 },
                    new Beer { Id = 4, Name = "Lager", Description = "A crisp and clean beer with a smooth finish.", AlcoholContent = 4.5 },
                    new Beer { Id = 5, Name = "Wheat Beer", Description = "A light and fruity beer made with wheat malt.", AlcoholContent = 4.8 },
                    new Beer { Id = 6, Name = "Pilsner", Description = "A pale lager with a light body and a slightly bitter taste.", AlcoholContent = 4.9 },
                    new Beer { Id = 7, Name = "Porter", Description = "A dark beer with a rich and malty flavor.", AlcoholContent = 5.5 },
                    new Beer { Id = 8, Name = "Saison", Description = "A farmhouse ale with a fruity and spicy character.", AlcoholContent = 6.0 },
                    new Beer { Id = 9, Name = "Amber Ale", Description = "A medium-bodied ale with a balanced malt and hop profile.", AlcoholContent = 5.2 },
                    new Beer { Id = 10, Name = "Blonde Ale", Description = "A light and easy-drinking ale with a mild hop flavor.", AlcoholContent = 4.7 }
                );
        }
    }
}
