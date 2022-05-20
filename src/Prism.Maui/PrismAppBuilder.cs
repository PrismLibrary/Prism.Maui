using Prism.AppModel;
using Prism.Behaviors;
using Prism.Events;
using Prism.Extensions;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;

namespace Prism;

public sealed class PrismAppBuilder<TApp> : PrismAppBuilder
    where TApp : Application
{
    public PrismAppBuilder(IContainerExtension containerExtension, MauiAppBuilder builder)
        : base(containerExtension, builder)
    {
        builder.UseMauiApp<TApp>();
    }
}

public abstract class PrismAppBuilder
{
    private List<Action<IContainerRegistry>> _registrations { get; }
    private List<Action<IContainerProvider>> _initializations { get; }
    private IContainerProvider _container { get; }
    private Action<IContainerProvider, INavigationService> _onAppStarted;

    protected PrismAppBuilder(IContainerExtension containerExtension, MauiAppBuilder builder)
    {
        if(containerExtension is null)
            throw new ArgumentNullException(nameof(containerExtension));

        _container = containerExtension;
        _registrations = new List<Action<IContainerRegistry>>();
        _initializations = new List<Action<IContainerProvider>>();

        MauiBuilder = builder;
        MauiBuilder.ConfigureContainer(new PrismServiceProviderFactory(RegistrationCallback));

        ContainerLocator.ResetContainer();
        ContainerLocator.SetContainerExtension(() => containerExtension);

        containerExtension.RegisterInstance(this);
        containerExtension.RegisterSingleton<IMauiInitializeService, PrismInitializationService>();

        ConfigureViewModelLocator(containerExtension);
    }

    public MauiAppBuilder MauiBuilder { get; }

    private void ConfigureViewModelLocator(IContainerProvider container)
    {
        ViewModelLocationProvider2.SetDefaultViewToViewModelTypeResolver(view =>
        {
            if (view is not BindableObject bindable)
                return null;

            return bindable.GetValue(ViewModelLocator.ViewModelProperty) as Type;
        });

        ViewModelLocationProvider2.SetDefaultViewModelFactory((view, type) =>
        {
            var overrides = new List<(Type Type, object Instance)>();
            if (container.IsRegistered<IResolverOverridesHelper>())
            {
                var resolver = container.Resolve<IResolverOverridesHelper>();
                var resolverOverrides = resolver.GetOverrides();
                if (resolverOverrides.Any())
                    overrides.AddRange(resolverOverrides);
            }

            if (!overrides.Any(x => x.Type == typeof(INavigationService)))
            {
                var navService = CreateNavigationService(container, view);
                overrides.Add((typeof(INavigationService), navService));
            }

            return container.Resolve(type, overrides.ToArray());
        });
    }

    public PrismAppBuilder RegisterTypes(Action<IContainerRegistry> registerTypes)
    {
        _registrations.Add(registerTypes);
        return this;
    }

    public PrismAppBuilder OnInitialized(Action<IContainerProvider> action)
    {
        _initializations.Add(action);
        return this;
    }

    internal void OnInitialized()
    {
        _initializations.ForEach(action => action(_container));

        if (_container.IsRegistered<IModuleCatalog>() && _container.Resolve<IModuleCatalog>().Modules.Any())
        {
            var manager = _container.Resolve<IModuleManager>();
            manager.Run();
        }

        var app = _container.Resolve<IApplication>();
        if (!NavigationRegistry.IsRegistered(nameof(NavigationPage)))
            ((IContainerRegistry)_container).RegisterForNavigation<NavigationPage>();
        if (!NavigationRegistry.IsRegistered(nameof(TabbedPage)))
            ((IContainerRegistry)_container).RegisterForNavigation<TabbedPage>();

        if (app is ILegacyPrismApplication prismApp)
            prismApp.OnInitialized();

        if (_onAppStarted is not null)
            _onAppStarted(_container, _container.Resolve<INavigationService>());
    }

    public MauiAppBuilder OnAppStart(Action<IContainerProvider, INavigationService> onAppStarted)
    {
        _onAppStarted = onAppStarted;
        return MauiBuilder;
    }

    /// <summary>
    /// Configures the <see cref="ViewModelLocator"/> used by Prism.
    /// </summary>
    public PrismAppBuilder ConfigureDefaultViewModelFactory(Func<IContainerProvider, object, Type, object> viewModelFactory)
    {
        ViewModelLocationProvider2.SetDefaultViewModelFactory((view, type) =>
            viewModelFactory(_container, view, type));

        return this;
    }

    public MauiApp Build()
    {
        return MauiBuilder.Build();
    }

    private void RegistrationCallback(IContainerExtension container)
    {
        RegisterDefaultRequiredTypes(container);

        _registrations.ForEach(action => action(container));
    }

    private void RegisterDefaultRequiredTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<IEventAggregator, EventAggregator>();
        containerRegistry.RegisterSingleton<IKeyboardMapper, KeyboardMapper>();
        containerRegistry.RegisterSingleton<IPageDialogService, PageDialogService>();
        //containerRegistry.RegisterSingleton<IDialogService, DialogService>();
        //containerRegistry.RegisterSingleton<IDeviceService, DeviceService>();
        containerRegistry.RegisterSingleton<IPageBehaviorFactory, PageBehaviorFactory>();
        containerRegistry.RegisterScoped<INavigationService, PageNavigationService>();
    }

    private INavigationService CreateNavigationService(IContainerProvider container, object view)
    {
        if (view is Page page)
        {
            return Navigation.Xaml.Navigation.GetNavigationService(page);
        }
        else if (view is VisualElement visualElement && visualElement.TryGetParentPage(out var parent))
        {
            return Navigation.Xaml.Navigation.GetNavigationService(parent);
        }

        return container.Resolve<INavigationService>();
    }
}
