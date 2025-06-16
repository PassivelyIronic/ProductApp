using System.Windows.Input;
using ProductApp.Models;
using ProductApp.Services;

namespace ProductApp.ViewModels
{
    [QueryProperty(nameof(ProductId), "productId")]
    public class ProductDetailViewModel : BaseViewModel
    {
        private readonly ProductService _productService;
        private Product _product;
        private bool _isLoading;
        private string _productId;

        public ProductDetailViewModel()
        {
            _productService = new ProductService();
            BackCommand = new Command(async () => await Shell.Current.GoToAsync("//ProductList"));
        }

        public string ProductId
        {
            get => _productId;
            set
            {
                SetProperty(ref _productId, value);
                _ = LoadProduct();
            }
        }

        public Product Product
        {
            get => _product;
            set => SetProperty(ref _product, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand BackCommand { get; }

        private async Task LoadProduct()
        {
            if (string.IsNullOrEmpty(ProductId) || IsLoading) return;

            IsLoading = true;

            try
            {
                Product = await _productService.GetProductByIdAsync(ProductId);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", $"Nie można załadować produktu: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}