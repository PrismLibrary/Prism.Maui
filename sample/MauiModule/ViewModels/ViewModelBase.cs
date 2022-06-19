using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace MauiModule.ViewModels;

public abstract class ViewModelBase : BindableBase, IInitialize, INavigatedAware, IPageLifecycleAware
{
    protected INavigationService _navigationService { get; }
    protected IPageDialogService _pageDialogs { get; }

    protected ViewModelBase(BaseServices baseServices)
    {
        _navigationService = baseServices.NavigationService;
        _pageDialogs = baseServices.PageDialogs;
        Title = Regex.Replace(GetType().Name, "ViewModel", string.Empty);
        Id = Guid.NewGuid().ToString();
        NavigateCommand = new DelegateCommand<string>(OnNavigateCommandExecuted);
        ShowPageDialog = new DelegateCommand(OnShowPageDialog);
        Messages = new ObservableCollection<string>();
        Messages.CollectionChanged += (sender, args) =>
        {
            foreach (string message in args.NewItems)
                Console.WriteLine($"{Title} - {message}");
        };
    }

    public string Title { get; }

    public string Id { get; }

    public ObservableCollection<string> Messages { get; }

    public DelegateCommand<string> NavigateCommand { get; }

    public DelegateCommand ShowPageDialog { get; }

    private void OnNavigateCommandExecuted(string uri)
    {
        Messages.Add($"OnNavigateCommandExecuted: {uri}");
        _navigationService.NavigateAsync(uri)
            .OnNavigationError(ex => Console.WriteLine(ex));
    }

    private void OnShowPageDialog()
    {
        Messages.Add("OnShowPageDialog");
        _pageDialogs.DisplayAlertAsync("Message", $"Hello from {Title}. This is a Page Dialog Service Alert!", "Ok");
    }

    public void Initialize(INavigationParameters parameters)
    {
        Messages.Add("ViewModel Initialized");
        foreach (var parameter in parameters.Where(x => x.Key.Contains("message")))
            Messages.Add(parameter.Value.ToString());
    }

    public void OnNavigatedFrom(INavigationParameters parameters)
    {
        Messages.Add("ViewModel NavigatedFrom");
    }

    public void OnNavigatedTo(INavigationParameters parameters)
    {
        Messages.Add("ViewModel NavigatedTo");
    }

    public void OnAppearing()
    {
        Messages.Add("View Appearing");
    }

    public void OnDisappearing()
    {
        Messages.Add("View Disappearing");
    }
}
