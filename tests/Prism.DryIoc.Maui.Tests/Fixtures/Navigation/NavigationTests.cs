using Prism.Controls;
using Prism.DryIoc.Maui.Tests.Mocks.ViewModels;
using Prism.DryIoc.Maui.Tests.Mocks.Views;

namespace Prism.DryIoc.Maui.Tests.Fixtures.Navigation;

public class NavigationTests
{
    [Theory]
    [InlineData("NavigationPage/MockViewA/MockViewB/MockViewC")]
    [InlineData("MockHome/NavigationPage/MockViewA")]
    public void PagesInjectScopedInstanceOfIPageAccessor(string uri)
    {
        var mauiApp = CreateBuilder(prism => prism.OnAppStart(navigation => navigation.NavigateAsync(uri)))
            .Build();
        var app = mauiApp.Services.GetRequiredService<IApplication>() as Application;
        var window = app!.Windows.First();

        var rootPage = window.Page;

        if(rootPage is FlyoutPage flyoutPage)
        {
            TestPage(flyoutPage);
            rootPage = flyoutPage.Detail;
        }

        TestPage(rootPage!);

        foreach (var page in rootPage!.Navigation.NavigationStack)
        {
            TestPage(page);
        }
    }

    private void TestPage(Page page)
    {
        Assert.NotNull(page.BindingContext);
        if(page.Parent is not null)
        {
            Assert.False(page.BindingContext == page);
            Assert.False(page.BindingContext == page.Parent);
            Assert.False(page.BindingContext == page.Parent.BindingContext);
        }

        if (page is NavigationPage)
        {
            Assert.IsType<PrismNavigationPage>(page);
            return;
        }

        var viewModel = page.BindingContext as MockViewModelBase;
        Assert.NotNull(viewModel);
        Assert.Same(page, viewModel!.Page);
    }

    private MauiAppBuilder CreateBuilder(Action<PrismAppBuilder> configurePrism) =>
        MauiApp.CreateBuilder()
            .UsePrismApp<Application>(prism =>
            {
                prism.RegisterTypes(container =>
                {
                    container.RegisterForNavigation<MockHome, MockHomeViewModel>()
                        .RegisterForNavigation<MockViewA, MockViewAViewModel>()
                        .RegisterForNavigation<MockViewB, MockViewBViewModel>()
                        .RegisterForNavigation<MockViewC, MockViewCViewModel>();
                });
                configurePrism(prism);
            });
}
