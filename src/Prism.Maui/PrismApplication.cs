using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation;

namespace Prism
{
    public abstract class PrismApplication : Application, IApplication
    {
        private readonly ObservableCollection<IWindow> _windows = new ObservableCollection<IWindow>();
        private IContainerExtension _containerExtension;
        private IModuleCatalog _moduleCatalog;

        /// <summary>
        /// The dependency injection container used to resolve objects
        /// </summary>
        protected IContainerProvider Container => _containerExtension;

        public new IReadOnlyList<IWindow> Windows => _windows;

        protected INavigationService NavigationService { get; private set; }

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
        }

        /// <summary>
        /// Configures the <see cref="ViewModelLocator"/> used by Prism.
        /// </summary>
        protected virtual void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                List<(Type Type, object Instance)> overrides = new List<(Type, object)>();
                if (Container.IsRegistered<IResolverOverridesHelper>())
                {
                   var resolver = Container.Resolve<IResolverOverridesHelper>();
                   var resolverOverrides = resolver.GetOverrides();
                   if (resolverOverrides.Any())
                       overrides.AddRange(resolverOverrides);
                }

                if (!overrides.Any(x => x.Type == typeof(INavigationService)))
                {
                    //var navService = CreateNavigationService(view);
                    var navService = Container.Resolve<INavigationService>();
                    overrides.Add((typeof(INavigationService), navService));
                }

                return Container.Resolve(type, overrides.ToArray());
            });
        }

        protected virtual void Initialize()
        {
            //PlatformInitializer?.RegisterTypes(_containerExtension);
            RegisterTypes(_containerExtension);

            _moduleCatalog = Container.Resolve<IModuleCatalog>();
            ConfigureModuleCatalog(_moduleCatalog);

            _containerExtension.CreateScope();
            NavigationService = _containerExtension.Resolve<INavigationService>();

            InitializeModules();
        }

        protected abstract void RegisterTypes(IContainerRegistry containerRegistry);
        protected abstract void OnWindowCreated(IActivationState activationState);

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
            var window = CreateWindow(activationState);
            _windows.Add(window);
            OnWindowCreated(activationState);
            return window;
        }

        protected override IWindow CreateWindow(IActivationState activationState)
        {
            return new PrismApplicationWindow();
        }
    }
}
