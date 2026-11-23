using BeerListApp.Api.Models;
using BeerListApp.Api.Services;
using Microsoft.AspNetCore.Identity;

namespace BeerListApp.Api.Endpoints
{
    public static class LoginEndpoints
    {
        public static void MapLoginEndpoints(this IEndpointRouteBuilder app)
        {
            var logger = app.ServiceProvider
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger(typeof(LoginEndpoints).FullName!);

            var group = app.MapGroup("api")
                .WithTags("Authentication");

            // POST: api/login
            // Authenticates a user and returns a JWT token.
            group.MapPost("/login", async Task<IResult> (
                LoginRequest request,
                UserManager<IdentityUser> userManager,
                SignInManager<IdentityUser> signInManager,
                JwtTokenService jwtTokenService) =>
            {
                // Validation request
                if (string.IsNullOrEmpty(request.Username) ||
                    string.IsNullOrEmpty(request.Password))
                {
                    return Results.BadRequest("Username and password are required.");
                }

                // Find user
                var user = await userManager.FindByNameAsync(request.Username);
                if (user is null)
                {
                    logger.LogWarning("Login failed for user {Username}: User not found.", request.Username);
                    return Results.Unauthorized();
                }

                // Validate password and account state
                var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
                if (!result.Succeeded)
                {
                    logger.LogWarning("Login failed for user {Username}: Invalid password or account locked.", request.Username);
                    return Results.Unauthorized();
                }

                // Do not bypass two-factor authentication
                if (await userManager.GetTwoFactorEnabledAsync(user))
                {
                    logger.LogWarning("Login failed for user {Username}: Two-factor authentication is enabled.", request.Username);
                    return Results.Unauthorized();
                }

                // Generate JWT token
                var tokenResponse = await jwtTokenService.CreateTokenAsync(user);

                logger.LogInformation("User {Username} logged in successfully.", request.Username);

                return Results.Ok(tokenResponse);
            })
            .AllowAnonymous()
            .RequireRateLimiting("login"); // Apply rate limiting to the login endpoint
        }
    }
}
