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
        /// Add a new beer to the database.
        /// </summary>
        Task<int> AddBeerAsync(Beer beer);
    }
}
