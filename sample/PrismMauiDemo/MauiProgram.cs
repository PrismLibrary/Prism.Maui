using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Prism;
using Prism.Ioc;

namespace PrismMauiDemo
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = PrismApp.CreateBuilder()
                .ConfigureModuleCatalog(moduleCatalog =>
                {
                    // Register Modules
                })
                .RegisterTypes(containerRegistry =>
                {
                    containerRegistry.RegisterForNavigation<MainPage>();
                })
                .UsePrismApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            return builder.Build();
        }
    }
}