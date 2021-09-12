using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;
using Prism.AppModel;
using Prism.Behaviors;
using Prism.Events;
using Prism.Extensions;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Microsoft.Maui.Controls.Hosting;

namespace Prism
{
    public abstract class PrismApplicationBase : Application, IApplication, IStartup
    {
        private readonly ObservableCollection<IWindow> _windows = new();
        private readonly IContainerExtension _containerExtension;
        private IModuleCatalog _moduleCatalog;

        /// <summary>
        /// The dependency injection container used to resolve objects
        /// </summary>
        protected IContainerProvider Container => _containerExtension;

        public new IReadOnlyList<IWindow> Windows => _windows;

        protected INavigationService NavigationService { get; private set; }

        protected PrismApplicationBase()
{
            ContainerLocator.SetContainerExtension(CreateContainerExtension);
            _containerExtension = ContainerLocator.Current;
            InitializeInternal();
        }

        void IStartup.Configure(IAppHostBuilder builder)
        {
            builder
                .UseMauiApp(x => this)
                .UseServiceProviderFactory(new PrismServiceProviderFactory(_containerExtension));
            Configure(builder);
        }

        protected virtual void Configure(IAppHostBuilder builder)
        {
        }

        /// <summary>
        /// Run the initialization process.
        /// </summary>
        private void InitializeInternal()
        {
            ConfigureViewModelLocator();
            Initialize();
        }

        protected abstract IContainerExtension CreateContainerExtension();

        /// <summary>
        /// Configures the <see cref="ViewModelLocator"/> used by Prism.
        /// </summary>
        protected virtual void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
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

            return Container.Resolve<INavigationService>();
        }

        protected virtual void Initialize()
        {
            RegisterRequiredServices(_containerExtension);
            RegisterTypes(_containerExtension);

            _moduleCatalog = Container.Resolve<IModuleCatalog>();
            ConfigureModuleCatalog(_moduleCatalog);

            InitializeModules();
        }

        protected virtual void RegisterRequiredServices(IContainerRegistry containerRegistry)
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

        protected abstract void RegisterTypes(IContainerRegistry containerRegistry);

        /// <summary>
        /// Used to Navigate
        /// </summary>
        /// <param name="activationState"></param>
        protected abstract Task OnWindowCreated(IActivationState activationState);

        /// <summary>
        /// Configures the <see cref="IModuleCatalog"/> used by Prism.
        /// </summary>
        /// <param name="moduleCatalog">The ModuleCatalog to configure</param>
        protected virtual void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) { }

        /// <summary>
        /// Initializes the modules.
        /// </summary>
        protected virtual void InitializeModules()
        {
            if (_moduleCatalog.Modules.Any())
            {
                IModuleManager manager = Container.Resolve<IModuleManager>();
                manager.Run();
            }
        }

        IWindow IApplication.CreateWindow(IActivationState activationState)
        {
            // We need to delay creating the Navigation Service since it
            // requires the IApplication which cannot be resolved from a
            // method called by the ctor...
            var scope = _containerExtension.CreateScope();
            NavigationService = scope.Resolve<INavigationService>();
            var window = CreateWindow(activationState);
            _windows.Add(window);
            OnWindowCreated(activationState).Wait();
            return window;
        }
    }
}
