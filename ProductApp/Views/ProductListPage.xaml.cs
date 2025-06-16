using ProductApp.ViewModels;

namespace ProductApp.Views
{
    public partial class ProductListPage : ContentPage
    {
        public ProductListPage(ProductListViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}