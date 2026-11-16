using MauiApp_StudyProject_.Models;

namespace MauiApp_StudyProject_.Services
{
    public class BeerServiceSelector : IBeerSelectorService
    {
        private readonly IBeerService _dbService;
        private readonly IBeerApiService _apiService;

        public BeerServiceSelector(
            IBeerService dbService,
            IBeerApiService apiService)
        {
            _dbService = dbService;
            _apiService = apiService;
        }

        public async Task<List<Beer>> GetBeersAsync()
        {
            return await UseApiAsync()
                ? await _apiService.GetBeersAsync()
                : await _dbService.GetBeersAsync();
        }

        public async Task<Beer?> GetBeerByIdAsync(int beerId)
        {
            return await UseApiAsync()
                ? await _apiService.GetBeerByIdAsync(beerId)
                : await _dbService.GetBeerByIdAsync(beerId);
        }

        public async Task<Beer> AddBeerAsync(Beer beer)
        {
            if (!await UseApiAsync())
                return await _dbService.AddBeerAsync(beer);

            return await _apiService.AddBeerAsync(beer);
        }

        public async Task UpdateBeerAsync(Beer beer)
        {
            if (await UseApiAsync())
                await _apiService.UpdateBeerAsync(beer.Id, beer);
            else
                await _dbService.UpdateBeerAsync(beer);
        }

        public async Task DeleteBeerAsync(Beer beer)
        {
            if (await UseApiAsync())
                await _apiService.DeleteBeerAsync(beer.Id);
            else
                await _dbService.DeleteBeerAsync(beer);
        }

        public async Task<bool> UseApiAsync()
        {
            return await _apiService.IsAvailableAsync();
        }
    }
}
