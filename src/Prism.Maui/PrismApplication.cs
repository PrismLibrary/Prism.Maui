using System.Collections.ObjectModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Prism.Extensions;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Navigation;

namespace Prism
{
    public abstract class PrismApplication : Application, IApplication
    {
        private readonly ObservableCollection<IWindow> _windows = new();
        private readonly IContainerExtension _containerExtension;

        /// <summary>
        /// The dependency injection container used to resolve objects
        /// </summary>
        protected IContainerProvider Container => _containerExtension;

        public new IReadOnlyList<IWindow> Windows => _windows;

        protected INavigationService NavigationService { get; private set; }

        protected PrismApplication()
        {
            _containerExtension = ContainerLocator.Current;
            ConfigureViewModelLocator();
        }

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

        /// <summary>
        /// Used to Navigate
        /// </summary>
        /// <param name="activationState"></param>
        protected abstract Task OnWindowCreated(IActivationState activationState);

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

        protected override Window CreateWindow(IActivationState activationState)
        {
            return new Window();
        }
    }
}
