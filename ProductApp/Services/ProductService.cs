using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Text.Json;
using ProductApp.Models;

namespace ProductApp.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:4000";
        private readonly CacheService _cacheService;
        private readonly ConnectivityService _connectivityService;
        private readonly Timer _syncTimer;
        private const int SYNC_INTERVAL_MINUTES = 5;
        public ProductService(CacheService cacheService, ConnectivityService connectivityService)
        {
            _httpClient = new HttpClient();
            _cacheService = cacheService;
            _connectivityService = connectivityService;

            // Automatyczna synchronizacja co 5 minut
            _syncTimer = new Timer(async _ => await SyncProductsIfNeeded(),
                                  null,
                                  TimeSpan.Zero,
                                  TimeSpan.FromMinutes(SYNC_INTERVAL_MINUTES));
        }

        public async Task<ObservableCollection<Product>> GetProductsAsync()
        {
            var products = new ObservableCollection<Product>();

            try
            {
                // Sprawdź czy jesteś online i cache jest nieaktualny
                if (_connectivityService.IsConnected &&
                    !await _cacheService.IsCacheValidAsync(TimeSpan.FromMinutes(10)))
                {
                    // Pobierz z API i zapisz do cache
                    var response = await _httpClient.GetStringAsync($"{BaseUrl}/products");
                    var productList = JsonSerializer.Deserialize<List<Product>>(response, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (productList != null)
                    {
                        await _cacheService.SaveProductsAsync(productList);
                        foreach (var product in productList)
                        {
                            products.Add(product);
                        }
                    }
                }
                else
                {
                    // Pobierz z cache
                    var cachedProducts = await _cacheService.GetCachedProductsAsync();
                    foreach (var cached in cachedProducts)
                    {
                        products.Add(new Product
                        {
                            Id = cached.Id,
                            Name = cached.Name,
                            Price = cached.Price
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Jeśli nie można pobrać z API, spróbuj cache
                try
                {
                    var cachedProducts = await _cacheService.GetCachedProductsAsync();
                    foreach (var cached in cachedProducts)
                    {
                        products.Add(new Product
                        {
                            Id = cached.Id,
                            Name = cached.Name,
                            Price = cached.Price
                        });
                    }
                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine($"Błąd pobierania produktów: {ex.Message}");
                }
            }

            return products;
        }


        public async Task<Product> GetProductByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{BaseUrl}/products/{id}");
                return JsonSerializer.Deserialize<Product>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd pobierania produktu {id}: {ex.Message}");
                return null;
            }
        }

        private async Task SyncProductsIfNeeded()
        {
            if (_connectivityService.IsConnected &&
                !await _cacheService.IsCacheValidAsync(TimeSpan.FromMinutes(SYNC_INTERVAL_MINUTES)))
            {
                try
                {
                    await GetProductsAsync(); // To wywoła synchronizację
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Błąd synchronizacji: {ex.Message}");
                }
            }
        }
    }
}
