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

        protected PrismAppBuilder(IContainerExtension containerExtension, MauiAppBuilder builder)
        {
            if(containerExtension is null)
                throw new ArgumentNullException(nameof(containerExtension));

            _registrations = new List<Action<IContainerRegistry>>();
            _initializations = new List<Action<IContainerProvider>>();

            MauiBuilder = builder;
            MauiBuilder.Host.UseServiceProviderFactory(new PrismServiceProviderFactory(RunInitializations));

            ContainerLocator.ResetContainer();
            ContainerLocator.SetContainerExtension(() => containerExtension);
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

        public MauiApp Build()
        {
            return MauiBuilder.Build();
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
