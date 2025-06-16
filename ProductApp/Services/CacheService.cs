using SQLite;
using ProductApp.Models;

namespace ProductApp.Services
{
    public class CacheService
    {
        private SQLiteAsyncConnection _database;
        private readonly string _databasePath;

        public CacheService()
        {
            _databasePath = Path.Combine(FileSystem.AppDataDirectory, "ProductCache.db3");
        }

        private async Task InitializeAsync()
        {
            if (_database != null) return;

            _database = new SQLiteAsyncConnection(_databasePath);
            await _database.CreateTableAsync<CachedProduct>();
        }

        public async Task<List<CachedProduct>> GetCachedProductsAsync()
        {
            await InitializeAsync();
            return await _database.Table<CachedProduct>().ToListAsync();
        }

        public async Task SaveProductsAsync(IEnumerable<Product> products)
        {
            await InitializeAsync();

            var cachedProducts = products.Select(p => new CachedProduct
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CachedAt = DateTime.Now
            });

            await _database.DeleteAllAsync<CachedProduct>();
            await _database.InsertAllAsync(cachedProducts);
        }

        public async Task<DateTime?> GetLastCacheTimeAsync()
        {
            await InitializeAsync();
            var lastProduct = await _database.Table<CachedProduct>()
                .OrderByDescending(p => p.CachedAt)
                .FirstOrDefaultAsync();

            return lastProduct?.CachedAt;
        }

        public async Task<bool> IsCacheValidAsync(TimeSpan maxAge)
        {
            var lastCache = await GetLastCacheTimeAsync();
            return lastCache.HasValue && DateTime.Now - lastCache.Value < maxAge;
        }
    }
}