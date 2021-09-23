using MauiModule;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;
using Prism;
using Prism.Ioc;
using Prism.Modularity;
using PrismMauiDemo.Views;

namespace PrismMauiDemo
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder()
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
}