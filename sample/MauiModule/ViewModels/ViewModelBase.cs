using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Prism.AppModel;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;

namespace MauiModule.ViewModels
{
    public abstract class ViewModelBase : IInitialize, INavigatedAware, IPageLifecycleAware
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
            Messages = new ObservableCollection<string>();
        }

        public string Title { get; }

        public string Id { get; }

        public ObservableCollection<string> Messages { get; }

        public DelegateCommand<string> NavigateCommand { get; }

        private void OnNavigateCommandExecuted(string uri)
        {
            _navigationService.NavigateAsync(uri)
                .OnNavigationError(ex => Console.WriteLine(ex));
        }

        public void Initialize(INavigationParameters parameters)
        {
            Messages.Add("ViewModel Initialized");
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
}
