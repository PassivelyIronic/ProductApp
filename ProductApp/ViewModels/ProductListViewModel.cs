using System.Collections.ObjectModel;
using System.Windows.Input;
using ProductApp.Models;
using ProductApp.Services;

namespace ProductApp.ViewModels
{
    public class ProductListViewModel : BaseViewModel
    {
        private readonly ProductService _productService;
        private readonly CartService _cartService;
        private ObservableCollection<Product> _products;
        private bool _isLoading;

        public ProductListViewModel(ProductService productService, CartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
            Products = new ObservableCollection<Product>();
            LoadProductsCommand = new Command(async () => await LoadProducts());
            ProductSelectedCommand = new Command<Product>(async (product) => await OnProductSelected(product));

            _ = LoadProducts();
        }

        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand LoadProductsCommand { get; }
        public ICommand ProductSelectedCommand { get; }

        private async Task LoadProducts()
        {
            if (IsLoading) return;

            IsLoading = true;

            try
            {
                var products = await _productService.GetProductsAsync();
                Products.Clear();

                foreach (var product in products)
                {
                    Products.Add(product);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", $"Nie można załadować produktów: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task OnProductSelected(Product product)
        {
            if (product == null) return;

            await Shell.Current.GoToAsync($"//ProductDetail?productId={product.Id}");
        }
    }
}