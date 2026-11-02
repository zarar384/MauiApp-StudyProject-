using MauiApp_StudyProject_.Models;

namespace MauiApp_StudyProject_.Services
{
    public interface IBeerService
    {
        /// <summary>
        /// Gets a list of all beers from the database.
        /// </summary>
        Task<List<Beer>> GetBeersAsync();

        /// <summary>
        /// Get beer by Id
        /// </summary>
        Task<Beer> GetBeerByIdAsync(int beerId);

        /// <summary>
        /// Add a new beer to the database.
        /// </summary>
        Task<int> AddBeerAsync(Beer beer);

        /// <summary>
        /// Update an existing beer in the database.
        /// </summary>
        Task<int> UpdateBeerAsync(Beer beer);

        /// <summary>
        /// Delete a beer from the database.
        /// </summary>
        Task<int> DeleteBeerAsync(Beer beer);
    }
}
