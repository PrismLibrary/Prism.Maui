using Prism.DryIoc.Maui.Tests.Mocks.ViewModels;
using Prism.DryIoc.Maui.Tests.Mocks.Views;
using Prism.Navigation.Xaml;

namespace Prism.DryIoc.Maui.Tests.Fixtures.Regions;

public class RegionFixture
{
    [Fact]
    public void ContentRegion_CreatedBy_RequestNavigate()
    {
        var mauiApp = MauiApp.CreateBuilder()
            .UsePrismApp<Application>(prism =>
                prism.RegisterTypes(container =>
                {
                    container.RegisterForNavigation<MockContentRegionPage, MockContentRegionPageViewModel>();
                    container.RegisterForRegionNavigation<MockRegionViewA, MockRegionViewAViewModel>();
                })
                .OnAppStart("MockContentRegionPage"))
            .Build();

        var app = mauiApp.Services.GetRequiredService<IApplication>() as Application;

        Assert.Single(app!.Windows);
        var window = app.Windows.First();
        Assert.NotNull(window.Page);

        Assert.IsType<MockContentRegionPage>(window.Page);
        var page = window.Page as MockContentRegionPage;
        Assert.NotNull(page.ContentRegion.Content);
        Assert.IsType<MockRegionViewA>(page.ContentRegion.Content);
        Assert.IsType<MockRegionViewAViewModel>(page.ContentRegion.Content.BindingContext);
    }

    [Fact]
    public void FrameRegion_CreatedBy_RegisterViewWithRegion()
    {
        var mauiApp = MauiApp.CreateBuilder()
            .UsePrismApp<Application>(prism =>
                prism.RegisterTypes(container =>
                {
                    container.RegisterForNavigation<MockContentRegionPage, MockContentRegionPageViewModel>();
                    container.RegisterForRegionNavigation<MockRegionViewA, MockRegionViewAViewModel>();
                })
                .OnInitialized(container =>
                {
                    var regionManager = container.Resolve<IRegionManager>();
                    regionManager.RegisterViewWithRegion("FrameRegion", "MockRegionViewA");
                })
                .OnAppStart("MockContentRegionPage"))
            .Build();

        var app = mauiApp.Services.GetRequiredService<IApplication>() as Application;

        Assert.Single(app!.Windows);
        var window = app.Windows.First();
        Assert.NotNull(window.Page);

        Assert.IsType<MockContentRegionPage>(window.Page);
        var page = window.Page as MockContentRegionPage;
        Assert.NotNull(page.FrameRegion.Content);
        Assert.IsType<MockRegionViewA>(page.FrameRegion.Content);
        Assert.IsType<MockRegionViewAViewModel>(page.FrameRegion.Content.BindingContext);
    }

    [Fact]
    public void RegionsShareContainer_WithPage()
    {
        var mauiApp = MauiApp.CreateBuilder()
            .UsePrismApp<Application>(prism =>
                prism.RegisterTypes(container =>
                {
                    container.RegisterForNavigation<MockContentRegionPage, MockContentRegionPageViewModel>();
                    container.RegisterForRegionNavigation<MockRegionViewA, MockRegionViewAViewModel>();
                })
                .OnInitialized(container =>
                {
                    var regionManager = container.Resolve<IRegionManager>();
                    regionManager.RegisterViewWithRegion("FrameRegion", "MockRegionViewA");
                })
                .OnAppStart("MockContentRegionPage"))
            .Build();

        var app = mauiApp.Services.GetRequiredService<IApplication>() as Application;

        Assert.Single(app!.Windows);
        var window = app.Windows.First();
        Assert.NotNull(window.Page);

        Assert.IsType<MockContentRegionPage>(window.Page);
        var page = window.Page as MockContentRegionPage;

        var regionManager = mauiApp.Services.GetRequiredService<IRegionManager>();
        var regions = regionManager.Regions.Cast<ITargetAwareRegion>();
        Assert.Equal(2, regions.Count());
        foreach (var region in regions)
        {
            Assert.Same(page.GetContainerProvider(), region.Container);
        }
    }

    [Fact]
    public void RegionViewModel_HasPageAccessor_WithCorrectPage()
    {
        // This validates that the NavigationService is using the correct Page to navigate from
        var mauiApp = MauiApp.CreateBuilder()
            .UsePrismApp<Application>(prism =>
                prism.RegisterTypes(container =>
                {
                    container.RegisterForNavigation<MockContentRegionPage, MockContentRegionPageViewModel>();
                    container.RegisterForRegionNavigation<MockRegionViewA, MockRegionViewAViewModel>();
                })
                .OnAppStart("MockContentRegionPage"))
            .Build();

        var regionManager = mauiApp.Services.GetRequiredService<IRegionManager>();
        var region = regionManager.Regions.First(x => x.Name == "ContentRegion");
        var activeView = region.ActiveViews.First();
        Assert.IsType<MockRegionViewA>(activeView);
        var viewModel = activeView.BindingContext as MockRegionViewAViewModel;
        Assert.NotNull(viewModel);

        Assert.NotNull(viewModel.Page);
        Assert.IsType<MockContentRegionPage>(viewModel.Page);
    }

    [Fact]
    public void RegionManager_HasTwoRegions()
    {
        var mauiApp = MauiApp.CreateBuilder()
            .UsePrismApp<Application>(prism => 
                prism.RegisterTypes(container =>
                {
                    container.RegisterForNavigation<MockContentRegionPage, MockContentRegionPageViewModel>();
                })
                .OnAppStart("MockContentRegionPage"))
            .Build();

        var regionManager = mauiApp.Services.GetRequiredService<IRegionManager>();
        Assert.Equal(2, regionManager.Regions.Count());
    }

    [Fact]
    public void PageHas_2_ChildViews()
    {
        var mauiApp = MauiApp.CreateBuilder()
            .UsePrismApp<Application>(prism =>
                prism.RegisterTypes(container =>
                {
                    container.RegisterForNavigation<MockContentRegionPage, MockContentRegionPageViewModel>();
                    container.RegisterForRegionNavigation<MockRegionViewA, MockRegionViewAViewModel>();
                })
                .OnInitialized(container =>
                {
                    var regionManager = container.Resolve<IRegionManager>();
                    regionManager.RegisterViewWithRegion("FrameRegion", "MockRegionViewA");
                })
                .OnAppStart("MockContentRegionPage"))
            .Build();

        var app = mauiApp.Services.GetRequiredService<IApplication>() as Application;

        Assert.Single(app!.Windows);
        var window = app.Windows.First();
        Assert.NotNull(window.Page);

        Assert.IsType<MockContentRegionPage>(window.Page);
        var page = window.Page as MockContentRegionPage;

        var children = page.GetChildRegions();
        Assert.NotNull(children);

        Assert.Equal(2, children.Count());
    }

    [Fact]
    public void RegionWithDefaultView_IsAutoPopulated()
    {
        var mauiApp = MauiApp.CreateBuilder()
            .UsePrismApp<Application>(prism =>
                prism.RegisterTypes(container =>
                {
                    container.RegisterForNavigation<MockPageWithRegionAndDefaultView>("MainPage");
                    container.RegisterForRegionNavigation<MockRegionViewA, MockRegionViewAViewModel>();
                })
                .OnAppStart("MainPage", ex => Assert.Null(ex)))
            .Build();

        var app = mauiApp.Services.GetRequiredService<IApplication>() as Application;

        Assert.Single(app!.Windows);
        var window = app.Windows.First();
        Assert.NotNull(window.Page);

        Assert.IsType<MockPageWithRegionAndDefaultView>(window.Page);
        var page = window.Page as MockPageWithRegionAndDefaultView;

        var region = page.Content as ContentView;

        Assert.NotNull(region.Content);
        Assert.IsType<MockRegionViewA>(region.Content);
    }
}
