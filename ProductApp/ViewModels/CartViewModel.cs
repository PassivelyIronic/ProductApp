using System.Collections.ObjectModel;
using System.Windows.Input;
using ProductApp.Models;
using ProductApp.Services;

namespace ProductApp.ViewModels
{
    public class CartViewModel : BaseViewModel
    {
        private readonly CartService _cartService;
        private decimal _totalPrice;
        private int _totalItems;

        public CartViewModel(CartService cartService)
        {
            _cartService = cartService;
            _cartService.CartChanged += OnCartChanged;

            RemoveFromCartCommand = new Command<CartItem>(async (item) => await RemoveFromCart(item));
            UpdateQuantityCommand = new Command<CartItem>(async (item) => await UpdateQuantity(item));
            ClearCartCommand = new Command(async () => await ClearCart());

            UpdateTotals();
        }

        public ObservableCollection<CartItem> CartItems => _cartService.CartItems;

        public decimal TotalPrice
        {
            get => _totalPrice;
            set => SetProperty(ref _totalPrice, value);
        }

        public int TotalItems
        {
            get => _totalItems;
            set => SetProperty(ref _totalItems, value);
        }

        public ICommand RemoveFromCartCommand { get; }
        public ICommand UpdateQuantityCommand { get; }
        public ICommand ClearCartCommand { get; }

        private void OnCartChanged(object sender, EventArgs e)
        {
            UpdateTotals();
        }

        private void UpdateTotals()
        {
            TotalPrice = _cartService.GetTotalPrice();
            TotalItems = _cartService.GetTotalItemsCount();
        }

        private async Task RemoveFromCart(CartItem item)
        {
            if (item == null) return;

            var result = await Application.Current.MainPage.DisplayAlert(
                "Potwierdzenie",
                $"Czy chcesz usunąć {item.ProductName} z koszyka?",
                "Tak", "Nie");

            if (result)
            {
                _cartService.RemoveFromCart(item.Id);
            }
        }

        private async Task UpdateQuantity(CartItem item)
        {
            if (item == null) return;

            var result = await Application.Current.MainPage.DisplayPromptAsync(
                "Ilość",
                $"Podaj nową ilość dla {item.ProductName}:",
                "OK", "Anuluj",
                item.Quantity.ToString(),
                keyboard: Keyboard.Numeric);

            if (int.TryParse(result, out int newQuantity))
            {
                _cartService.UpdateQuantity(item.Id, newQuantity);
            }
        }

        private async Task ClearCart()
        {
            var result = await Application.Current.MainPage.DisplayAlert(
                "Potwierdzenie",
                "Czy chcesz wyczyścić cały koszyk?",
                "Tak", "Nie");

            if (result)
            {
                _cartService.ClearCart();
            }
        }
    }
}