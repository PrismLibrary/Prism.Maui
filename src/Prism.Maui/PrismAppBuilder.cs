using System.ComponentModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Prism.AppModel;
using Prism.Behaviors;
using Prism.Events;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Services;

namespace Prism
{
    public abstract class PrismAppBuilder
    {
        private List<Action<IContainerRegistry>> _registrations { get; }
        private List<Action<IContainerProvider>> _initializations { get; }

        protected PrismAppBuilder()
            : this(null)
        {
            ContainerLocator.SetContainerExtension(CreateContainerExtension);
        }

        protected PrismAppBuilder(IContainerExtension containerExtension)
        {
            _registrations = new List<Action<IContainerRegistry>>();
            _initializations = new List<Action<IContainerProvider>>();

            Builder = MauiApp.CreateBuilder(); ;
            Builder.Host.UseServiceProviderFactory(new PrismServiceProviderFactory(RunInitializations));

            if(containerExtension != null)
                ContainerLocator.SetContainerExtension(() => containerExtension);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public MauiAppBuilder Builder { get; }

        protected abstract IContainerExtension CreateContainerExtension();

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

        public MauiAppBuilder UsePrismApp<TApp>()
            where TApp : PrismApplication
        {
            return Builder
                .UseMauiApp<TApp>();
        }

        private void RunInitializations(IContainerExtension container)
        {
            RegisterDefaultRequiredTypes(container);

            _registrations.ForEach(action => action(container));
            _initializations.ForEach(action => action(container));
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
}
