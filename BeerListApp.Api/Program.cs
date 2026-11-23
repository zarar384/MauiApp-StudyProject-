
using System.IdentityModel.Tokens.Jwt;
using System.Threading.RateLimiting;
using BeerListApp.Api.Endpoints;
using BeerListApp.Api.Options;
using BeerListApp.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

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

            // Add CORS policy to allow requests from the configured client origin
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    var origin = builder.Configuration["Cors:AllowedOrigin"]
                        ?? throw new InvalidOperationException("CORS origin is missing.");

                    policy.WithOrigins(origin) // Allow requests only from the configured client origin
                          .AllowAnyMethod()    // Allow any HTTP method (GET, POST, PUT, DELETE, etc.)
                          .AllowAnyHeader();   // Allow any HTTP headers (Authorization, Content-Type, etc.)
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

                // Configure account lockout after multiple failed login attempts
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<BeerListAppDbContext>()
            .AddSignInManager();

            // Load and validate JWT configuration
            var jwtOptions = builder.Configuration
                .GetSection(JwtOptions.SectionName)
                .Get<JwtOptions>()
                ?? throw new InvalidOperationException("JWT configuration is missing.");

            if (string.IsNullOrWhiteSpace(jwtOptions.Issuer) ||
                string.IsNullOrWhiteSpace(jwtOptions.Audience) ||
                string.IsNullOrWhiteSpace(jwtOptions.Key) ||
                jwtOptions.AccessTokenMinutes <= 0)
            {
                throw new InvalidOperationException("JWT configuration is invalid.");
            }

            byte[] signingKey;

            try
            {
                signingKey = Convert.FromBase64String(jwtOptions.Key);
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException("JWT key must be Base64 encoded.", ex);
            }

            if (signingKey.Length < 32)
            {
                throw new InvalidOperationException("JWT signing key must be at least 256 bits.");
            }

            // Register JWT configuration and token generation service
            builder.Services.AddSingleton(jwtOptions);
            builder.Services.AddScoped<JwtTokenService>();

            // Add JWT Bearer authentication
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;

                    // Configure JWT token validation parameters
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(signingKey),

                        NameClaimType = JwtRegisteredClaimNames.UniqueName,
                        RoleClaimType = "role",

                        ClockSkew = TimeSpan.FromSeconds(30)
                    };
                });

            // Add rate limiting to protect the login endpoint from excessive requests
            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // Limit login requests to 5 attempts per minute per IP address
                options.AddPolicy("login", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        }));
            });

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

            // Configure request routing and CORS
            app.UseRouting();
            app.UseCors();

            // Enable rate limiting for endpoints with configured policies
            app.UseRateLimiter();

            // Enable JWT authentication and authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Map the login minimal API endpoints
            app.MapLoginEndpoints();

            // Map the beer minimal API endpoints
            app.MapBeerEndpoints();

            app.Run();
        }
    }
}