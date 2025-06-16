using ProductApp.ViewModels;

namespace ProductApp.Views
{
    public partial class CartPage : ContentPage
    {
        public CartPage(CartViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}