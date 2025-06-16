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
        private readonly CartService _cartService;
        private int _quantity = 1;

        public ProductDetailViewModel(ProductService productService, CartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
            BackCommand = new Command(async () => await Shell.Current.GoToAsync("//ProductList"));
            AddToCartCommand = new Command(async () => await AddToCart());
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
            set
            {
                SetProperty(ref _product, value);
                // Poinformuj o zmianie stanu przycisku
                ((Command)AddToCartCommand).ChangeCanExecute();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public int Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        public ICommand BackCommand { get; }
        public ICommand AddToCartCommand { get; }

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

        private async Task AddToCart()
        {
            if (Product == null || Quantity <= 0) return;

            _cartService.AddToCart(Product, Quantity);

            await Application.Current.MainPage.DisplayAlert(
                "Sukces",
                $"Dodano {Product.Name} (x{Quantity}) do koszyka!",
                "OK");
        }
    }
}