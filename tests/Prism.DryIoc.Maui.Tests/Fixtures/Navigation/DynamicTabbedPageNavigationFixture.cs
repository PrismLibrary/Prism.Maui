﻿using Prism.Controls;
using Prism.DryIoc.Maui.Tests.Mocks.Views;

namespace Prism.DryIoc.Maui.Tests.Fixtures.Navigation;

public class DynamicTabbedPageNavigationFixture : TestBase
{
    public DynamicTabbedPageNavigationFixture(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Fact]
    public void CreatesTabs_WithSingleContentPage()
    {
        var mauiApp = CreateBuilder(prism => prism.OnAppStart(navigation =>
            navigation.CreateBuilder()
                .AddTabbedSegment(t =>
                    t.CreateTab("MockViewA")
                     .CreateTab("MockViewB"))
                .Navigate())).Build();
        var app = mauiApp.Services.GetRequiredService<IApplication>() as Application;
        Assert.Single(app.Windows);
        var window = app.Windows[0];
        Assert.IsType<TabbedPage>(window.Page);
        var tabbedPage = window.Page as TabbedPage;

        Assert.Equal(2, tabbedPage.Children.Count);
        Assert.IsType<MockViewA>(tabbedPage.Children[0]);
        Assert.IsType<MockViewB>(tabbedPage.Children[1]);

        Assert.Same(tabbedPage.Children[0], tabbedPage.CurrentPage);
    }

    [Fact]
    public void CreatesTabs_WithNavigationPageAndContentPage()
    {
        var mauiApp = CreateBuilder(prism => prism.OnAppStart(navigation =>
            navigation.CreateBuilder()
                .AddTabbedSegment(t =>
                    t.CreateTab(ct => ct.AddNavigationPage().AddSegment("MockViewA"))
                     .CreateTab(ct => ct.AddNavigationPage().AddSegment("MockViewB")))
                .Navigate())).Build();
        var app = mauiApp.Services.GetRequiredService<IApplication>() as Application;
        Assert.Single(app.Windows);
        var window = app.Windows[0];
        Assert.IsType<TabbedPage>(window.Page);
        var tabbedPage = window.Page as TabbedPage;

        Assert.Equal(2, tabbedPage.Children.Count);
        Assert.IsType<PrismNavigationPage>(tabbedPage.Children[0]);
        var tab0 = tabbedPage.Children[0] as NavigationPage;
        Assert.IsType<MockViewA>(tab0.CurrentPage);
        Assert.IsType<PrismNavigationPage>(tabbedPage.Children[1]);
        var tab1 = tabbedPage.Children[1] as NavigationPage;
        Assert.IsType<MockViewB>(tab1.CurrentPage);

        Assert.Same(tabbedPage.Children[0], tabbedPage.CurrentPage);
    }
}