
namespace BeerListApp.Api
{
    public class Program
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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseCors();

            //app.MapGet("/endpoint", (HttpContext httpContext) =>
            //{
            //})
            //.WithName("EndpointName");

            app.Run();
        }
    }
}
