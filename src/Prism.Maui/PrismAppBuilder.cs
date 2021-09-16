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
        private bool registeredRequiredTypes;
        private List<Action<IContainerRegistry>> _registrations { get; }
        private List<Action<IContainerProvider>> _initializations { get; }

        protected PrismAppBuilder(MauiAppBuilder builder)
        {
            _registrations = new List<Action<IContainerRegistry>>();
            _initializations = new List<Action<IContainerProvider>>();

            Builder = builder;
            ContainerLocator.SetContainerExtension(CreateContainerExtension);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public MauiAppBuilder Builder { get; }

        protected abstract IContainerExtension CreateContainerExtension();

        public PrismAppBuilder RegisterRequiredTypes(Action<IContainerRegistry> registerRequiredTypes)
        {
            registeredRequiredTypes = true;
            _registrations.Add(registerRequiredTypes);
            return this;
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

        public MauiAppBuilder UsePrismApp<TApp>()
            where TApp : PrismApplication
        {
            var container = ContainerLocator.Current;
            if (!registeredRequiredTypes)
                RegisterDefaultRequiredTypes(container);

            _registrations.ForEach(action => action(container));

            Builder.Host.UseServiceProviderFactory(new PrismServiceProviderFactory(_initializations));
            return Builder
                .UseMauiApp<TApp>();
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
            containerRegistry.RegisterScoped<INavigationService, PageNavigationService>();
        }
    }
}
