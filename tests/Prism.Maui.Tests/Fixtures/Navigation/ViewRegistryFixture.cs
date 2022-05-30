using System;
using System.Linq;
using Microsoft.Maui.Controls;
using Prism.Behaviors;
using Prism.Common;
using Prism.Maui.Tests.Mocks.Ioc;
using Prism.Maui.Tests.Mocks.ViewModels;
using Prism.Maui.Tests.Mocks.Views;
using Prism.Navigation.Xaml;

namespace Prism.Maui.Tests.Fixtures.Navigation;

public class ViewRegistryFixture
{
    [Fact]
    public void ViewIsRegisteredWithRegistry()
    {
        var container = new TestContainer();
        container.RegisterForNavigation<ContentPageMock>("ContentPage");

        var registry = container.Resolve<NavigationRegistry>();
        Assert.True(registry.IsRegistered("ContentPage"));
    }

    [Fact]
    public void ViewIsNotRegisteredWithRegistry()
    {
        var container = new TestContainer();

        var registry = container.Resolve<NavigationRegistry>();
        Assert.False(registry.IsRegistered("FooBar"));
    }

    [Fact]
    public void RegistryHas4Registrations()
    {
        var container = new TestContainer();
        container.RegisterForNavigation<PageMock>();
        container.RegisterForNavigation<ContentPageMock>("ContentPage");
        container.RegisterForNavigation<ContentPageMock1>("ContentPage1");
        container.RegisterForNavigation<SecondContentPageMock>("SecondContentPageMock");

        var registry = container.Resolve<NavigationRegistry>();
        Assert.Equal(4, registry.Registrations.Count());
    }

    [Fact]
    public void CreateView_ViewModelProperty_IsSet()
    {
        var container = new TestContainer();
        container.RegisterInstance(new PageAccessor());
        container.RegisterForNavigation<ContentPageMock, ContentPageMockViewModel>();

        var registry = container.Resolve<NavigationRegistry>();
        var page = registry.CreateView(container, "ContentPageMock") as Page;

        Assert.NotNull(page);
        var viewModelType = page.GetValue(ViewModelLocator.ViewModelProperty) as Type;
        Assert.NotNull(viewModelType);
        Assert.Equal(typeof(ContentPageMockViewModel), viewModelType);
    }

    [Fact]
    public void CreateView_ContainerProperty_IsSet()
    {
        var container = new TestContainer();
        container.RegisterInstance(new PageAccessor());
        container.RegisterForNavigation<PageMock, PageMockViewModel>();

        var registry = container.Resolve<NavigationRegistry>();
        var page = registry.CreateView(container, "PageMock") as Page;

        Assert.NotNull(page);
        var attachedContainer = page.GetContainerProvider();
        Assert.NotNull(attachedContainer);
    }

    [Fact]
    public void CreateView_BindingContext_IsSet()
    {
        var container = new TestContainer();
        container.RegisterInstance(new PageAccessor());
        container.RegisterForNavigation<PageMock, PageMockViewModel>();

        var registry = container.Resolve<NavigationRegistry>();
        var page = registry.CreateView(container, "PageMock") as Page;

        Assert.NotNull(page);
        Assert.NotNull(page.BindingContext);
        Assert.IsType<PageMockViewModel>(page.BindingContext);
    }

    [Fact]
    public void CreateView_AppliesPageBehaviors()
    {
        var container = new TestContainer();
        container.RegisterInstance(new PageAccessor());
        container.RegisterForNavigation<PageMock, PageMockViewModel>();
        container.RegisterPageBehavior<PageLifeCycleAwareBehavior>();

        var registry = container.Resolve<NavigationRegistry>();
        var page = registry.CreateView(container, "PageMock") as Page;

        Assert.NotNull(page);
        Assert.Single(page.Behaviors);
        Assert.IsType<PageLifeCycleAwareBehavior>(page.Behaviors.First());
    }
}
