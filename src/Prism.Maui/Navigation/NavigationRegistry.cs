using System.ComponentModel;
using System.Data;
using Prism.Behaviors;
using Prism.Common;
using Prism.Ioc;
using Prism.Mvvm;

namespace Prism.Navigation;

public static class NavigationRegistry
{
    private static readonly List<ViewRegistration> _registrations = new ();

    public static void Register<TView, TViewModel>(string name) =>
        Register(typeof(TView), typeof(TViewModel), name);

    public static void Register(Type viewType, Type viewModelType, string name)
    {
        if (_registrations.Any(x => x.Name == name))
            throw new DuplicateNameException($"A view with the name '{name}' has already been registered");

        var registration = new ViewRegistration
        {
            View = viewType,
            ViewModel = viewModelType,
            Name = name
        };
        _registrations.Add(registration);
    }

    public static object CreateView(IContainerProvider container, string name)
    {
        try
        {
            var registration = _registrations.FirstOrDefault(x => x.Name == name);
            if (registration is null)
                throw new KeyNotFoundException($"No view with the name '{name}' has been registered");

            var view = container.Resolve(registration.View) as BindableObject;

            view.SetValue(Xaml.Navigation.NavigationScopeProperty, container);

            if (view is Page page)
            {
                var behaviors = container.Resolve<IPageBehaviorFactory>();
                ConfigurePage(container, page, behaviors);
            }

            if (view.BindingContext is not null)
                return view;

            if (registration.ViewModel is not null)
                view.SetValue(ViewModelLocator.ViewModelProperty, registration.ViewModel);

            ViewModelLocator.Autowire(view);

            return view;
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Unable to create page '{name}'.", ex);
        }
    }

    public static Type GetPageType(string name) =>
        _registrations.FirstOrDefault(x => x.Name == name)?.View;

    public static ViewRegistration GetPageNavigationInfo(Type viewType) => 
        _registrations.FirstOrDefault(x => x.View == viewType);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ClearRegistrationCache() => _registrations.Clear();

    private static void ConfigurePage(IContainerProvider container, Page page, IPageBehaviorFactory behaviors)
    {
        if(page is TabbedPage tabbed)
        {
            foreach(var child in tabbed.Children)
            {
                var scope = container.CreateScope();
                ConfigurePage(scope, child, behaviors);
            }
        }
        else if(page is NavigationPage navPage && navPage.RootPage is not null)
        {
            var scope = container.CreateScope();
            ConfigurePage(scope, navPage.RootPage, behaviors);
        }

        if (page.GetValue(Xaml.Navigation.NavigationScopeProperty) is null)
            page.SetValue(Xaml.Navigation.NavigationScopeProperty, container);

        var navService = container.Resolve<INavigationService>();
        if (navService is IPageAware pa)
            pa.Page = page;

        page.SetValue(Xaml.Navigation.NavigationServiceProperty, navService);

        behaviors.ApplyPageBehaviors(page);
    }
}
