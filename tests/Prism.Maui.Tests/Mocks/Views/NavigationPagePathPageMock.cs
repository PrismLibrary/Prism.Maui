using Prism.Common;
using Prism.Maui.Tests.Mocks;
using Prism.Maui.Tests.Navigation.Mocks.ViewModels;
using Microsoft.Maui.Controls;

namespace Prism.Maui.Tests.Navigation.Mocks.Views
{
    public class NavigationPathPageMock : ContentPage
    {
        public NavigationPathPageMockViewModel ViewModel { get; }
        public NavigationPathPageMock()
        {
            var navService = new PageNavigationServiceMock(null, new ApplicationMock(), null);
            ((IPageAware)navService).Page = this;

            BindingContext = ViewModel = new NavigationPathPageMockViewModel(navService);
        }
    }

    public class NavigationPathPageMock2 : NavigationPathPageMock
    {
        public NavigationPathPageMock2()
        {

        }
    }

    public class NavigationPathPageMock3 : NavigationPathPageMock
    {
        public NavigationPathPageMock3()
        {

        }
    }

    public class NavigationPathPageMock4 : NavigationPathPageMock
    {
        public NavigationPathPageMock4()
        {

        }
    }

    public class NavigationPathTabbedPageMock : TabbedPage
    {
        public NavigationPathPageMockViewModel ViewModel { get; }

        public NavigationPathTabbedPageMock()
        {
            var navService = new PageNavigationServiceMock(null, new ApplicationMock(), null);
            ((IPageAware)navService).Page = this;

            BindingContext = ViewModel = new NavigationPathPageMockViewModel(navService);

            Children.Add(new NavigationPathPageMock());
            Children.Add(new NavigationPathPageMock2());
            Children.Add(new NavigationPathPageMock3());
        }
    }
}
