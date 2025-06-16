using ProductApp.Services;
using ProductApp.ViewModels;
using ProductApp.Views;

namespace ProductApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Dodaj NuGet package: sqlite-net-pcl

            // Rejestracja serwisów
            builder.Services.AddSingleton<ProductService>();
            builder.Services.AddSingleton<CartService>();
            builder.Services.AddSingleton<CacheService>();
            builder.Services.AddSingleton<ConnectivityService>();

            // Rejestracja ViewModels
            builder.Services.AddTransient<ProductListViewModel>();
            builder.Services.AddTransient<ProductDetailViewModel>();
            builder.Services.AddTransient<CartViewModel>();

            // Rejestracja Views
            builder.Services.AddTransient<ProductListPage>();
            builder.Services.AddTransient<ProductDetailPage>();
            builder.Services.AddTransient<CartPage>();

            return builder.Build();
        }
    }
}