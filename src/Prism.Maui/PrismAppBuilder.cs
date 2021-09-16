using System;
using Microsoft.Maui;
using Prism.AppModel;
using Prism.Behaviors;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation;
using Prism.Services;
using System.ComponentModel;

namespace Prism
{
    public abstract class PrismAppBuilder
    {
        private Action<IContainerRegistry> _registerRequiredTypes;

        protected PrismAppBuilder(MauiAppBuilder builder)
        {
            Builder = builder;
            ContainerLocator.SetContainerExtension(CreateContainerExtension);
            _registerRequiredTypes = RegisterDefaultRequiredTypes;
            //Builder.UseServiceProviderFactory(new PrismServiceProviderFactory(_createContainerExtensionDelegate));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public MauiAppBuilder Builder { get; }

        private IContainerExtension _container;
        public IContainerExtension Container => _container ??= ContainerLocator.Current;

        protected abstract IContainerExtension CreateContainerExtension();

        public PrismAppBuilder RegisterRequiredTypes(Action<IContainerRegistry> registerRequiredTypes)
        {
            _registerRequiredTypes = registerRequiredTypes;
            return this;
        }

        public MauiAppBuilder RegisterTypes(Action<IContainerRegistry> registerTypes)
        {
            _registerRequiredTypes(Container);
            registerTypes(Container);
            return Builder;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RegisterDefaultRequiredTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IEventAggregator, EventAggregator>();
            containerRegistry.RegisterSingleton<IKeyboardMapper, KeyboardMapper>();
            containerRegistry.RegisterSingleton<IPageDialogService, PageDialogService>();
            //containerRegistry.RegisterSingleton<IDialogService, DialogService>();
            //containerRegistry.RegisterSingleton<IDeviceService, DeviceService>();
            containerRegistry.RegisterSingleton<IPageBehaviorFactory, PageBehaviorFactory>();
            containerRegistry.RegisterSingleton<IModuleCatalog, ModuleCatalog>();
            containerRegistry.RegisterSingleton<IModuleManager, ModuleManager>();
            containerRegistry.RegisterSingleton<IModuleInitializer, ModuleInitializer>();
            containerRegistry.RegisterScoped<INavigationService, PageNavigationService>();
        }
    }
}
