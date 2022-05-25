using Prism.Common;

namespace MauiRegionsModule.ViewModels;

public abstract class RegionViewModelBase : BindableBase, IRegionAware
{
    protected string Name => GetType().Name.Replace("ViewModel", string.Empty);
    protected INavigationService _navigationService { get; }
    protected IRegionNavigationService? _regionNavigation { get; private set; }

    protected RegionViewModelBase(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public bool IsNavigationTarget(INavigationContext navigationContext) =>
        navigationContext.NavigatedName() == Name;

    private string? _message;
    public string? Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    public string? PageName => ((IPageAware)_navigationService).Page?.GetType()?.Name;

    public void OnNavigatedFrom(INavigationContext navigationContext)
    {

    }

    public void OnNavigatedTo(INavigationContext navigationContext)
    {
        if (navigationContext.Parameters.ContainsKey(nameof(Message)))
            Message = navigationContext.Parameters.GetValue<string>(nameof(Message));

        _regionNavigation = navigationContext.NavigationService;
    }
}
