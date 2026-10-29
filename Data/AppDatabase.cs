using MauiApp_StudyProject_.Models;
using SQLite;

namespace MauiApp_StudyProject_.Data
{
    public class AppDatabase
    {
        public SQLiteAsyncConnection Connection { get; }

        public AppDatabase()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "maui-test.db");

            Connection = new SQLiteAsyncConnection(dbPath);
        }

        public async Task InitializeAsync()
        {
            await Connection.CreateTableAsync<Beer>();

            // other tables
        }
    }
}