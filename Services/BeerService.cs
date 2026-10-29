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

        public Task<List<Beer>> GetBeersAsync()
        {
            return _db.Table<Beer>().ToListAsync();
        }

        public Task<int> AddBeerAsync(Beer beer)
        {
            return _db.InsertAsync(beer);
        }
    }
}
