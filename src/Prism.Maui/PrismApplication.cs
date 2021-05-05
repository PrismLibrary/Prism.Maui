using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Microsoft.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui.Controls;
using System.Reflection.PortableExecutable;

namespace Prism
{
    public abstract class PrismApplication : Application
    {
        private IContainerExtension _containerExtension;
        private IModuleCatalog _moduleCatalog;

        /// <summary>
        /// The dependency injection container used to resolve objects
        /// </summary>
        protected IContainerProvider Container => _containerExtension;

        protected PrismApplication(IContainerExtension container)
        {
            _containerExtension = container;
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

        protected virtual void Initialize()
        {
            RegisterRequiredTypes(_containerExtension);
            //PlatformInitializer?.RegisterTypes(_containerExtension);
            RegisterTypes(_containerExtension);

            _moduleCatalog = Container.Resolve<IModuleCatalog>();
            ConfigureModuleCatalog(_moduleCatalog);

            //_containerExtension.CreateScope();
            //NavigationService = _containerExtension.Resolve<INavigationService>();

            InitializeModules();
        }

        protected abstract void RegisterTypes(IContainerRegistry containerRegistry);
        protected abstract void OnInitialized();

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

        protected override IWindow CreateWindow(IActivationState activationState)
        {
            return new PrismApplicationWindow();
        }
    }
}
