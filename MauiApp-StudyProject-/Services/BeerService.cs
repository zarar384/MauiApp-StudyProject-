using MauiApp_StudyProject_.Data;
using MauiApp_StudyProject_.Models;
using SQLite;

namespace MauiApp_StudyProject_.Services
{
    public class BeerService : IBeerService
    {
        private readonly SQLiteAsyncConnection _db;

        public BeerService(AppDatabase db)
        {
            _db = db.Connection;
        }

        public async Task<List<Beer>> GetBeersAsync()
        {
            return await _db.Table<Beer>().ToListAsync();
        }

        public async Task<Beer> AddBeerAsync(Beer beer)
        {
            await _db.InsertAsync(beer);
            return beer;
        }

        public async Task DeleteBeerAsync(Beer beer)
        {
            await _db.DeleteAsync(beer);
        }

        public async Task<Beer?> GetBeerByIdAsync(int beerId)
        {
            return await _db.Table<Beer>()
                .FirstOrDefaultAsync(b => b.Id == beerId);
        }

        public async Task UpdateBeerAsync(Beer beer)
        {
            await _db.UpdateAsync(beer);
        }
    }
}
