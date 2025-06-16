using System.Collections.ObjectModel;
using ProductApp.Models;

namespace ProductApp.Services
{
    public class CartService
    {
        private ObservableCollection<CartItem> _cartItems = new();

        public ObservableCollection<CartItem> CartItems => _cartItems;

        public event EventHandler CartChanged;

        public void AddToCart(Product product, int quantity = 1)
        {
            var existingItem = _cartItems.FirstOrDefault(x => x.ProductId == product.Id);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                _cartItems.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductPrice = product.Price,
                    Quantity = quantity
                });
            }

            CartChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveFromCart(string cartItemId)
        {
            var item = _cartItems.FirstOrDefault(x => x.Id == cartItemId);
            if (item != null)
            {
                _cartItems.Remove(item);
                CartChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void UpdateQuantity(string cartItemId, int newQuantity)
        {
            var item = _cartItems.FirstOrDefault(x => x.Id == cartItemId);
            if (item != null)
            {
                if (newQuantity <= 0)
                {
                    RemoveFromCart(cartItemId);
                }
                else
                {
                    item.Quantity = newQuantity;
                    CartChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public decimal GetTotalPrice()
        {
            return _cartItems.Sum(x => x.TotalPrice);
        }

        public int GetTotalItemsCount()
        {
            return _cartItems.Sum(x => x.Quantity);
        }

        public void ClearCart()
        {
            _cartItems.Clear();
            CartChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}