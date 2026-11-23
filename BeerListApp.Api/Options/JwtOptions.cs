namespace BeerListApp.Api.Options
{
    public sealed class JwtOptions
    {
        public const string SectionName = "Jwt";

        /// <summary>
        /// Who issued the token. This is typically the URL of the issuing authority.
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Who the token is intended for. 
        /// This is typically the URL of the audience that will consume the token.
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// The secret key used to sign the JWT token.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// The duration in minutes for which the access token is valid.
        /// </summary>
        public int AccessTokenMinutes { get; set; } = 30;
    }
}
