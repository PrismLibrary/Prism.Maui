using System;
using System.ComponentModel;
using Prism.Behaviors;
using Prism.Events;
using Prism.Modularity;
using Prism.Navigation;

namespace Prism.Ioc
{
    public class ContainerOptionsBuilder
    {
        private Action<IContainerRegistry> _registerRequiredTypes;

        public ContainerOptionsBuilder()
        {

        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IContainerExtension Container { get; private set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetContainer(IContainerExtension container)
        {
            ContainerLocator.SetContainerExtension(() => container);
            Container = ContainerLocator.Current;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RegisterRequiredTypes()
        {
            if (_registerRequiredTypes is null)
                _registerRequiredTypes = DefaultRegisterRequiredTypes;

            _registerRequiredTypes(Container);
        }

        public void AppendRequriedTypes(Action<IContainerRegistry> appendDelegate)
        {
            _registerRequiredTypes = c =>
            {
                DefaultRegisterRequiredTypes(c);
                appendDelegate(c);
            };
        }

        public void RegisterRequiredTypes(Action<IContainerRegistry> registerDelegate)
        {
            _registerRequiredTypes = registerDelegate;
        }

        private static void DefaultRegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.RegisterSingleton<IApplicationProvider, ApplicationProvider>();
            //containerRegistry.RegisterSingleton<IApplicationStore, ApplicationStore>();
            containerRegistry.RegisterSingleton<IEventAggregator, EventAggregator>();
            //containerRegistry.RegisterSingleton<IKeyboardMapper, KeyboardMapper>();
            //containerRegistry.RegisterSingleton<IPageDialogService, PageDialogService>();
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
