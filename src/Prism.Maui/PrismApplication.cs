using Prism.Extensions;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Navigation;
using Application = Microsoft.Maui.Controls.Application;

namespace Prism;

public abstract class PrismApplication : Application, IPrismApplication
{
    private readonly IContainerExtension _containerExtension;

    /// <summary>
    /// The dependency injection container used to resolve objects
    /// </summary>
    protected IContainerProvider Container => _containerExtension;

    protected INavigationService NavigationService { get; private set; }

    protected PrismApplication()
    {
        _containerExtension = ContainerLocator.Current;
        ConfigureViewModelLocator();
        NavigationService = Container.Resolve<INavigationService>((typeof(IApplication), this));
    }

    /// <summary>
    /// Configures the <see cref="ViewModelLocator"/> used by Prism.
    /// </summary>
    protected virtual void ConfigureViewModelLocator()
    {
        ViewModelLocationProvider2.SetDefaultViewToViewModelTypeResolver(view =>
        {
            if (!(view is BindableObject bindable))
                return null;

            return bindable.GetValue(ViewModelLocator.ViewModelProperty) as Type;
        });
        ViewModelLocationProvider2.SetDefaultViewModelFactory((view, type) =>
        {
            var overrides = new List<(Type Type, object Instance)>();
            if (Container.IsRegistered<IResolverOverridesHelper>())
            {
               var resolver = Container.Resolve<IResolverOverridesHelper>();
               var resolverOverrides = resolver.GetOverrides();
               if (resolverOverrides.Any())
                   overrides.AddRange(resolverOverrides);
            }

            if (!overrides.Any(x => x.Type == typeof(INavigationService)))
            {
                var navService = CreateNavigationService(view);
                overrides.Add((typeof(INavigationService), navService));
            }

            return Container.Resolve(type, overrides.ToArray());
        });
    }

    void IPrismApplication.OnInitialized() => OnInitialized();

    protected abstract void OnInitialized();

    private INavigationService CreateNavigationService(object view)
    {
        if (view is Page page)
        {
            return Navigation.Xaml.Navigation.GetNavigationService(page);
        }
        else if (view is VisualElement visualElement && visualElement.TryGetParentPage(out var parent))
        {
            return Navigation.Xaml.Navigation.GetNavigationService(parent);
        }

        return Container.Resolve<INavigationService>((typeof(IApplication), this));
    }

    protected sealed override Window CreateWindow(IActivationState activationState) =>
        Windows.OfType<PrismWindow>().First(x => x.Name == PrismWindow.DefaultWindowName);
}
