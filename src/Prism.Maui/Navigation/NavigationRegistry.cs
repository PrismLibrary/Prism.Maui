﻿using Prism.Behaviors;
using Prism.Common;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Navigation.Xaml;
using TabbedPage = Microsoft.Maui.Controls.TabbedPage;

namespace Prism.Navigation;

internal class NavigationRegistry : ViewRegistryBase, INavigationRegistry
{
    public NavigationRegistry(IEnumerable<ViewRegistration> registrations)
        : base(ViewType.Page, registrations)
    {
    }

    protected override void ConfigureView(BindableObject bindable, IContainerProvider container)
    {
        ConfigurePage(container, bindable as Page);
    }

    private static void ConfigurePage(IContainerProvider container, Page page)
    {
        if (page is TabbedPage tabbed)
        {
            foreach (var child in tabbed.Children)
            {
                var scope = container.CreateScope();
                ConfigurePage(scope, child);
            }
        }
        else if (page is NavigationPage navPage && navPage.RootPage is not null)
        {
            var scope = container.CreateScope();
            ConfigurePage(scope, navPage.RootPage);
        }

        if (page.GetContainerProvider() is null)
            page.SetContainerProvider(container);

        var accessor = container.Resolve<IPageAccessor>();
        if (accessor.Page is not null && accessor.Page != page)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
#endif
            throw new NavigationException($"Invalid Scope provided. The current scope Page Accessor contains '{accessor.Page.GetType().FullName}', expected '{page.GetType().FullName}'.", page);
        }
        else if (accessor.Page is null)
            accessor.Page = page;

        var behaviorFactories = container.Resolve<IEnumerable<IPageBehaviorFactory>>();
        foreach (var factory in behaviorFactories)
            factory.ApplyPageBehaviors(page);
    }
}
