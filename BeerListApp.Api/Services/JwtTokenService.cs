
using BeerListApp.Api.Models;
using BeerListApp.Api.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BeerListApp.Api.Services
{
    public sealed class JwtTokenService
    {
        private readonly JwtOptions _options;
        private readonly UserManager<IdentityUser> _userManager;

        public JwtTokenService(
            JwtOptions options,
            UserManager<IdentityUser> userManager)
        {
            _options = options;
            _userManager = userManager;
        }

        public async Task<LoginResponse> CreateTokenAsync(
            IdentityUser user)
        {
            // Set token creation and expiration times
            var now = DateTime.UtcNow;
            var expiresAt = now.AddMinutes(
                _options.AccessTokenMinutes);

            // Get user roles from Identity
            var roles = await _userManager.GetRolesAsync(user);

            // Add user information and unique token identifier to JWT claims
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(
                    JwtRegisteredClaimNames.UniqueName,
                    user.UserName ?? user.Id),
                new(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString("N"))
            };

            // Add user roles to JWT claims
            claims.AddRange(
                roles.Select(role => new Claim("role", role)));

            // Create a security key from the configured JWT secret
            var key = new SymmetricSecurityKey(
                Convert.FromBase64String(_options.Key));

            // Configure JWT signing credentials using HMAC SHA-256
            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            // Generate JWT with issuer, audience, claims, and expiration
            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: expiresAt,
                signingCredentials: credentials);

            // Convert JWT to a string for the client
            var accessToken = new JwtSecurityTokenHandler()
                .WriteToken(token);

            // Return the access token and its expiration time
            return new LoginResponse(
                accessToken,
                "Bearer",
                expiresAt);
        }
    }
}
