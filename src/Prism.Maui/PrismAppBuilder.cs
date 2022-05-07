using Prism.AppModel;
using Prism.Behaviors;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation;
using Prism.Services;

namespace Prism;

public sealed class PrismAppBuilder<TApp> : PrismAppBuilder
    where TApp : PrismApplication
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
    }

    public MauiAppBuilder MauiBuilder { get; }

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
        if(_container.IsRegistered<IModuleCatalog>() && _container.Resolve<IModuleCatalog>().Modules.Any())
        {
            var manager = _container.Resolve<IModuleManager>();
            manager.Run();
        }

        _initializations.ForEach(action => action(_container));

        var app = _container.Resolve<IApplication>();
        if (app is IPrismApplication prismApp)
            prismApp.OnInitialized();
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
}
