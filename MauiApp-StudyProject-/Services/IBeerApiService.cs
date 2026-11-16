using MauiApp_StudyProject_.Models;

namespace MauiApp_StudyProject_.Services
{
    public interface IBeerApiService
    {
        /// <summary>
        /// Gets a list of all beers from the API.
        /// </summary>
        Task<List<Beer>> GetBeersAsync();

        /// <summary>
        /// Get beer by Id from the API.
        /// </summary>
        Task<Beer?> GetBeerByIdAsync(int beerId);

        /// <summary>
        /// Add a new beer to the API.
        /// </summary>
        Task<Beer> AddBeerAsync(Beer beer);

        /// <summary>
        /// Update an existing beer in the API.
        /// </summary>
        Task UpdateBeerAsync(int beerId, Beer beer);

        /// <summary>
        /// Delete a beer from the API.
        /// </summary>
        Task DeleteBeerAsync(int beerId);

        /// <summary>
        /// Check if the API is available by sending a HEAD request to the base URL.
        /// </summary>
        Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
    }
}
