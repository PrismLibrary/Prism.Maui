using MauiModule;
using MauiModule.ViewModels;
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
                containerRegistry.RegisterForNavigation<RootPage>();
                containerRegistry.RegisterForNavigation<SamplePage>();
            })
            .OnAppStart(navigationService => navigationService.CreateBuilder()
                .AddNavigationSegment("MainPage")
                .AddNavigationPage()
                .AddNavigationSegment<ViewAViewModel>()
                .AddNavigationSegment("ViewB")
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
