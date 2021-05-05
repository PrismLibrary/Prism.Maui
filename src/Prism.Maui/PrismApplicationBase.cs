using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui.Controls;

namespace Prism
{
    public abstract class PrismApplicationBase : IApplication, IStartup //, IHostApplicationLifetime
    {
        private IContainerExtension _containerExtension;
        private IModuleCatalog _moduleCatalog;
        private readonly List<IWindow> _windows = new List<IWindow>();

        /// <summary>
        /// The dependency injection container used to resolve objects
        /// </summary>
        public IContainerProvider Container => _containerExtension;

        public IReadOnlyList<IWindow> Windows => _windows;

        protected PrismApplicationBase()
        {
            InitializeInternal();
        }

        /// <summary>
        /// Run the initialization process.
        /// </summary>
        private void InitializeInternal()
        {
            ConfigureViewModelLocator();
            Initialize();
            OnInitialized();
        }

        /// <summary>
        /// Configures the <see cref="ViewModelLocator"/> used by Prism.
        /// </summary>
        protected virtual void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                List<(Type Type, object Instance)> overrides = new List<(Type, object)>();
                //if (Container.IsRegistered<IResolverOverridesHelper>())
                //{
                //    var resolver = Container.Resolve<IResolverOverridesHelper>();
                //    var resolverOverrides = resolver.GetOverrides();
                //    if (resolverOverrides.Any())
                //        overrides.AddRange(resolverOverrides);
                //}

                //if (!overrides.Any(x => x.Type == typeof(INavigationService)))
                //{
                //    var navService = CreateNavigationService(view);
                //    overrides.Add((typeof(INavigationService), navService));
                //}

                return Container.Resolve(type, overrides.ToArray());
            });
        }

        /// <summary>
        /// Run the bootstrapper process.
        /// </summary>
        protected virtual void Initialize()
        {
            ContainerLocator.SetContainerExtension(CreateContainerExtension);
            _containerExtension = ContainerLocator.Current;
            RegisterRequiredTypes(_containerExtension);
            //PlatformInitializer?.RegisterTypes(_containerExtension);
            RegisterTypes(_containerExtension);

            _moduleCatalog = Container.Resolve<IModuleCatalog>();
            ConfigureModuleCatalog(_moduleCatalog);

            //_containerExtension.CreateScope();
            //NavigationService = _containerExtension.Resolve<INavigationService>();

            InitializeModules();
        }

        void IStartup.Configure(IAppHostBuilder appBuilder)
        {
            appBuilder.ConfigureServices(ConfigureServices)
                .ConfigureFonts(ConfigureFonts)
                .ConfigureMauiHandlers(ConfigureMauiHandlers);

            Configure(appBuilder);

            appBuilder.UseMauiApp(x => this)
                .UseServiceProviderFactory(new PrismServiceProviderFactory(_containerExtension));
        }

        protected virtual void Configure(IAppHostBuilder appBuilder)
        {
        }

        protected virtual void ConfigureMauiHandlers(IMauiHandlersCollection handlersCollection)
        {
        }

        protected virtual void ConfigureFonts(HostBuilderContext host, IFontCollection fonts)
        {
        }

        protected virtual void ConfigureServices(HostBuilderContext host, IServiceCollection services)
        {
        }

        public virtual IWindow CreateWindow(IActivationState activationState)
        {
            return new PrismApplicationWindow
            {
                MauiContext = activationState.Context
            };
        }
        protected abstract void RegisterTypes(IContainerRegistry containerRegistry);
        protected abstract void OnInitialized();
        protected abstract IContainerExtension CreateContainerExtension();

        /// <summary>
        /// Registers all types that are required by Prism to function with the container.
        /// </summary>
        /// <param name="containerRegistry"></param>
        protected virtual void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.RegisterSingleton<IApplicationProvider, ApplicationProvider>();
            //containerRegistry.RegisterSingleton<IApplicationStore, ApplicationStore>();
            containerRegistry.RegisterSingleton<IEventAggregator, EventAggregator>();
            //containerRegistry.RegisterSingleton<IKeyboardMapper, KeyboardMapper>();
            //containerRegistry.RegisterSingleton<IPageDialogService, PageDialogService>();
            //containerRegistry.RegisterSingleton<IDialogService, DialogService>();
            //containerRegistry.RegisterSingleton<IDeviceService, DeviceService>();
            //containerRegistry.RegisterSingleton<IPageBehaviorFactory, PageBehaviorFactory>();
            containerRegistry.RegisterSingleton<IModuleCatalog, ModuleCatalog>();
            containerRegistry.RegisterSingleton<IModuleManager, ModuleManager>();
            containerRegistry.RegisterSingleton<IModuleInitializer, ModuleInitializer>();
            //containerRegistry.RegisterScoped<INavigationService, PageNavigationService>();
            //containerRegistry.Register<INavigationService, PageNavigationService>(NavigationServiceName);
        }

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
    }

    public class PrismApplicationWindow : Window
    {
        public PrismApplicationWindow()
            : base(null)
        {
        }

        public PrismApplicationWindow(Page page)
            : base(page)
        {
        }

        public IMauiContext MauiContext { get; set; }
    }

    //public class PrismApplicationWindow : IWindow
    //{
    //    public IMauiContext MauiContext { get; set; }
    //    public IPage Page { get; set; }
    //}
}
