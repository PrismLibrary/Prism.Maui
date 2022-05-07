using MauiModule;
using Prism;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation;
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
                containerRegistry.RegisterForNavigation<MainPage>();
                containerRegistry.RegisterForNavigation<SamplePage>();
                containerRegistry.RegisterForNavigation<NavigationPage>();
                containerRegistry.RegisterForNavigation<TabbedPage>();
            })
            .OnAppStart(async navigationService =>
            {
                var result = await navigationService.NavigateAsync("MainPage/NavigationPage/ViewB");
                if (!result.Success)
                {
                    System.Diagnostics.Debugger.Break();
                }
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            })
            .Build();
    }
}
