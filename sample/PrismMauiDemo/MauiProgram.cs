using MauiModule;
using MauiModule.ViewModels;
using PrismMauiDemo.ViewModels;
using PrismMauiDemo.Views;

namespace PrismMauiDemo;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        return MauiApp.CreateBuilder()
            .UsePrismApp<App>()
            .ConfigureModuleCatalog(moduleCatalog =>
            {
                moduleCatalog.AddModule<MauiAppModule>();
            })
            .RegisterTypes(containerRegistry =>
            {
                containerRegistry.RegisterGlobalNavigationObserver();
                containerRegistry.RegisterForNavigation<MainPage>();
                containerRegistry.RegisterForNavigation<RootPage>();
                containerRegistry.RegisterForNavigation<SamplePage>();
                containerRegistry.RegisterForNavigation<SplashPage>();
            })
            .AddGlobalNavigationObserver(context => context.Subscribe(x =>
            {
                if (x.Type == NavigationRequestType.Navigate)
                    Console.WriteLine($"Navigation: {x.Type} - {x.Uri}");
                else
                    Console.WriteLine($"Navigation: {x.Type}");

                var status = x.Cancelled ? "Cancelled" : x.Result.Success ? "Success" : "Failed";
                Console.WriteLine($"Result: {status}");
            }))
            .OnAppStart(navigationService => navigationService.CreateBuilder()
                .AddNavigationSegment<SplashPageViewModel>()
                .Navigate(HandleNavigationError))
            //.OnAppStart(async navigationService =>
            //{
                
            //    var result = await navigationService.NavigateAsync("MainPage/NavigationPage/ViewA/ViewB/ViewC/ViewD");
            //    if (!result.Success)
            //    {
            //        System.Diagnostics.Debugger.Break();
            //    }
            //})
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            })
            .Build();
    }

    private static void HandleNavigationError(Exception ex)
    {
        Console.WriteLine(ex);
        System.Diagnostics.Debugger.Break();
    }
}
