using System;
using Microsoft.Extensions.DependencyInjection;

namespace Prism.Ioc
{
    public class PrismServiceProviderFactory : IServiceProviderFactory<IContainerExtension>
    {
        private IContainerExtension _container { get; }

        public PrismServiceProviderFactory(IContainerExtension container)
        {
            _container = container;
        }

        public IContainerExtension CreateBuilder(IServiceCollection services)
        {
            _container.Populate(services);
            return _container;
        }

        public IServiceProvider CreateServiceProvider(IContainerExtension containerExtension)
        {
            return containerExtension.CreateServiceProvider();
        }
    }
}
