using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui;
using Prism.Ioc;

namespace Prism.Navigation
{
    public class PageNavigationService : INavigationService
    {
        private IApplication _app { get; }
        private IContainerProvider _container { get; }

        public PageNavigationService(IApplication application, IContainerProvider container)
        {
            _app = application;
            _container = container;
        }

        public Task<INavigationResult> GoBackAsync()
        {
            throw new NotImplementedException();
        }

        public Task<INavigationResult> GoBackAsync(INavigationParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<INavigationResult> GoBackAsync(INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            throw new NotImplementedException();
        }

        public Task<INavigationResult> GoBackToRootAsync(INavigationParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<INavigationResult> NavigateAsync(Uri uri)
        {
            throw new NotImplementedException();
        }

        public Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters parameters)
        {
            throw new NotImplementedException();
        }

        public async Task<INavigationResult> NavigateAsync(string name)
        {
            try
            {
                var page = NavigationRegistry.CreateView(_container, name) as IView;
                var window = _app.Windows.First();
                window.View = page;

                return new NavigationResult { Success = true };
            }
            catch (Exception ex)
            {
                return new NavigationResult { Exception = ex };
            }
        }

        public Task<INavigationResult> NavigateAsync(string name, INavigationParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<INavigationResult> NavigateAsync(string name, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            throw new NotImplementedException();
        }

        public Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            throw new NotImplementedException();
        }
    }
}