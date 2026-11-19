
using BeerListApp.Api.Endpoints;
using Microsoft.AspNetCore.Identity;

namespace BeerListApp.Api
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Add CORS policy to allow requests from any origin
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin() // Allow requests from any origin (e.g., http://localhost:3000, https://example.com, etc.)
                           .AllowAnyMethod() // Allow any HTTP method (GET, POST, PUT, DELETE, etc.)
                           .AllowAnyHeader(); // Allow any HTTP headers (Authorization, Content-Type, etc.)
                });
            });

            // Add the DbContext with SQLite configuration
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=BeerListApp.db";
            builder.Services.AddDbContext<BeerListAppDbContext>(options => options.UseSqlite(connectionString));

            // Add Identity services for authentication and authorization
            builder.Services.AddIdentityCore<IdentityUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 12;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<BeerListAppDbContext>();

            // Add Serilog for logging
            builder.Services.AddSerilog((services, logger) => logger
                .ReadFrom.Configuration(builder.Configuration) // Read configuration from appsettings.json
                .ReadFrom.Services(services)                   // Read configuration from registered services
                .Enrich.FromLogContext()                       // Enrich logs with contextual information
                .WriteTo.Console());                           // Write logs to the console

            var app = builder.Build();

            // Configure Serilog request logging
            app.UseSerilogRequestLogging();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();

                // Apply any pending migrations to the database
                var db = scope.ServiceProvider.GetRequiredService<BeerListAppDbContext>();
                db.Database.Migrate();

                app.MapOpenApi();
            }

            // Configure the HTTP request pipeline for non-development environments
            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseCors();
            app.UseAuthorization();

            // Map the beer minimal API endpoints
            app.MapBeerEndpoints();

            app.Run();
        }
    }
}