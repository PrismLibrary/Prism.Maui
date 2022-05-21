namespace MauiModule.ViewModels;

public class BaseServices
{
    public BaseServices(INavigationService navigationService, IPageDialogService pageDialogs)
    {
        NavigationService = navigationService;
        PageDialogs = pageDialogs;
    }

    public INavigationService NavigationService { get; }
    public IPageDialogService PageDialogs { get; }
}
