using MauiModule;
using MauiModule.ViewModels;
using MauiRegionsModule;
using PrismMauiDemo.ViewModels;
using PrismMauiDemo.Views;

namespace PrismMauiDemo;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        return MauiApp.CreateBuilder()
            .UsePrismApp<App>()
            .AddGlobalNavigationObserver(context => context.Subscribe(x =>
            {
                if (x.Type == NavigationRequestType.Navigate)
                    Console.WriteLine($"Navigation: {x.Type} - {x.Uri}");
                else
                    Console.WriteLine($"Navigation: {x.Type}");

                var status = x.Cancelled ? "Cancelled" : x.Result.Success ? "Success" : "Failed";
                Console.WriteLine($"Result: {status}");
            }))
            .ConfigureWithBootstrapper<Bootstrapper>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            })
            .Build();
    }
}
