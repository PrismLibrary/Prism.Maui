using MauiModule;
using Prism;
using Prism.Ioc;
using Prism.Modularity;
using PrismMauiDemo.Views;

namespace PrismMauiDemo;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
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
            .MauiBuilder
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        return builder.Build();
    }
}
