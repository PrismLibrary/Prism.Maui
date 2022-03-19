using Microsoft.Extensions.DependencyInjection;

namespace Prism.Ioc
{
    public class PrismServiceProviderFactory : IServiceProviderFactory<IContainerExtension>
    {
        private Action<IContainerExtension> _registerTypes { get; }

        public PrismServiceProviderFactory(Action<IContainerExtension> registerTypes)
        {
            _registerTypes = registerTypes;
        }

        public IContainerExtension CreateBuilder(IServiceCollection services)
        {
            var container = ContainerLocator.Current;
            container.Populate(services);
            _registerTypes(container);
            if (!container.IsRegistered(typeof(IServiceScopeFactory)))
                container.Register<IServiceScopeFactory, ServiceScopeFactory>();

            return container;
        }

        public IServiceProvider CreateServiceProvider(IContainerExtension containerExtension)
        {
            return containerExtension.CreateServiceProvider();
        }
    }

    internal class ServiceScopeFactory : IServiceScopeFactory
    {
        private IServiceProvider _services { get; }

        public ServiceScopeFactory(IServiceProvider services)
        {
            _services = services;
        }

        public IServiceScope CreateScope()
        {
            return _services.CreateScope();
        }
    }
}
