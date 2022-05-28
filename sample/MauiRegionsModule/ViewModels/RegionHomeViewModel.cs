namespace MauiRegionsModule.ViewModels;

internal class RegionHomeViewModel
{
    private INavigationService _navigationService { get; }

    public RegionHomeViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        NavigateCommand = new DelegateCommand<string>(OnNavigateCommandExecuted);
    }

    public DelegateCommand<string> NavigateCommand { get; }

    private void OnNavigateCommandExecuted(string uri)
    {
        _navigationService.NavigateAsync(uri);
    }
}

public class ContentRegionPageViewModel : IInitialize
{
    private IRegionManager _regionManager { get; }

    public ContentRegionPageViewModel(IRegionManager regionManager)
    {
        _regionManager = regionManager;
    }

    public void Initialize(INavigationParameters parameters)
    {
        _regionManager.RequestNavigate("ContentRegion", "RegionViewA");
    }
}
