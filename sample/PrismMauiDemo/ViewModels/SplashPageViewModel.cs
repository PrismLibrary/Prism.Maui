namespace PrismMauiDemo.ViewModels;

internal class SplashPageViewModel : IPageLifecycleAware
{
    private INavigationService _navigationService { get; }

    public SplashPageViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public async void OnAppearing()
    {
        await Task.Delay(1000);
        _navigationService.CreateBuilder()
            .AddSegment<RootPageViewModel>()
            .Navigate();
    }

    public void OnDisappearing()
    {

    }
}
