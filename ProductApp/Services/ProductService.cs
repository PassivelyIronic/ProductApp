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

        public ProductService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<ObservableCollection<Product>> GetProductsAsync()
        {
            var products = new ObservableCollection<Product>();

            try
            {
                var response = await _httpClient.GetStringAsync($"{BaseUrl}/products");
                var productList = JsonSerializer.Deserialize<List<Product>>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (productList != null)
                {
                    foreach (var product in productList)
                    {
                        products.Add(product);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd pobierania listy produktów: {ex.Message}");
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
    }
}
