using MauiModule;
using MauiModule.ViewModels;
using PrismMauiDemo.ViewModels;
using PrismMauiDemo.Views;

namespace PrismMauiDemo;

public class Bootstrapper : PrismBootstrapper
{
    public override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        moduleCatalog.AddModule<MauiAppModule>();
        moduleCatalog.AddModule<MauiTestRegionsModule>();
    }

    public override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterGlobalNavigationObserver();
        containerRegistry.RegisterForNavigation<MainPage>();
        containerRegistry.RegisterForNavigation<RootPage>();
        containerRegistry.RegisterForNavigation<SamplePage>();
        containerRegistry.RegisterForNavigation<SplashPage>();
    }

    public override async Task OnAppStart(IContainerProvider container, INavigationService navigationService)
    {
        await navigationService.CreateBuilder()
                .AddNavigationSegment<SplashPageViewModel>()
                .NavigateAsync(HandleNavigationError);
    }

    private void HandleNavigationError(Exception ex)
    {
        System.Diagnostics.Debugger.Break();
        Console.Error.WriteLine(ex);
    }
}
