using Prism.Controls;
using Prism.DryIoc.Maui.Tests.Mocks.ViewModels;
using Prism.DryIoc.Maui.Tests.Mocks.Views;
using Prism.Navigation.Xaml;

namespace Prism.DryIoc.Maui.Tests.Fixtures.Navigation;

public class NavigationTests : TestBase
{
    public NavigationTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Theory]
    [InlineData("NavigationPage/MockViewA/MockViewB/MockViewC")]
    [InlineData("MockHome/NavigationPage/MockViewA")]
    public void PagesInjectScopedInstanceOfIPageAccessor(string uri)
    {
        var mauiApp = CreateBuilder(prism => prism.OnAppStart(uri))
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

    [Fact]
    public async Task AddsPageFromRelativeURI()
    {
        var mauiApp = CreateBuilder(prism => prism.OnAppStart("NavigationPage/MockViewA"))
            .Build();
        var app = mauiApp.Services.GetRequiredService<IApplication>() as Application;
        var window = app!.Windows.First();

        var rootPage = window.Page as NavigationPage;
        Assert.NotNull(rootPage);
        TestPage(rootPage);
        var currentPage = rootPage.CurrentPage;
        Assert.IsType<MockViewA>(currentPage);
        TestPage(currentPage);
        var container = currentPage.GetContainerProvider();
        var navService = container.Resolve<INavigationService>();
        Assert.Single(rootPage.Navigation.NavigationStack);
        await navService.NavigateAsync("MockViewB");
        Assert.IsType<MockViewB>(rootPage.CurrentPage);
        TestPage(rootPage.CurrentPage);
        Assert.Equal(2, rootPage.Navigation.NavigationStack.Count);
    }

    [Fact]
    public async Task RelativeNavigation_RemovesPage_AndNavigates()
    {
        var mauiApp = CreateBuilder(prism => prism.OnAppStart("NavigationPage/MockViewA/MockViewB"))
            .Build();
        var app = mauiApp.Services.GetRequiredService<IApplication>() as Application;
        var window = app!.Windows.First();

        var rootPage = window.Page as NavigationPage;
        Assert.NotNull(rootPage);
        TestPage(rootPage);
        var currentPage = rootPage.CurrentPage;
        Assert.IsType<MockViewB>(currentPage);
        TestPage(currentPage);
        var container = currentPage.GetContainerProvider();
        var navService = container.Resolve<INavigationService>();
        Assert.Equal(2, rootPage.Navigation.NavigationStack.Count);
        await navService.NavigateAsync("../MockViewC");
        Assert.IsType<MockViewC>(rootPage.CurrentPage);
        TestPage(rootPage.CurrentPage);
        Assert.Equal(2, rootPage.Navigation.NavigationStack.Count);
    }

    [Fact]
    public async Task AbsoluteNavigation_ResetsWindowPage()
    {
        var mauiApp = CreateBuilder(prism => prism.OnAppStart("MockViewA"))
            .Build();
        var app = mauiApp.Services.GetRequiredService<IApplication>() as Application;
        var window = app!.Windows.First();

        var rootPage = window.Page as MockViewA;
        Assert.NotNull(rootPage);
        var container = rootPage.GetContainerProvider();
        var navService = container.Resolve<INavigationService>();
        var result = await navService.NavigateAsync("/MockViewB");
        Assert.True(result.Success);
        Assert.NotEqual(rootPage, window.Page);
    }

    [Fact]
    public async Task AddsModalPageFromRelativeURI()
    {
        var mauiApp = CreateBuilder(prism => prism.OnAppStart("MockViewA"))
            .Build();
        var app = mauiApp.Services.GetRequiredService<IApplication>() as Application;
        var window = app!.Windows.First();

        var rootPage = window.Page as MockViewA;
        Assert.NotNull(rootPage);
        Assert.IsType<MockViewA>(rootPage);
        var container = rootPage.GetContainerProvider();
        var navService = container.Resolve<INavigationService>();
        Assert.Empty(rootPage.Navigation.ModalStack);
        var result = await navService.NavigateAsync("MockViewB");
        Assert.True(result.Success);
        Assert.Single(rootPage.Navigation.ModalStack);
        Assert.IsType<MockViewB>(rootPage.Navigation.ModalStack.Last());
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
}
