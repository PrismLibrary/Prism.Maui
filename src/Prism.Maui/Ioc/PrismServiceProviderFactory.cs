using System;
using Microsoft.Extensions.DependencyInjection;

namespace Prism.Ioc
{
    public class PrismServiceProviderFactory : IServiceProviderFactory<IContainerExtension>
    {
        public IContainerExtension CreateBuilder(IServiceCollection services)
        {
            var container = ContainerLocator.Current;
            container.Populate(services);
            return container;
        }

        public IServiceProvider CreateServiceProvider(IContainerExtension containerExtension)
        {
            return containerExtension.CreateServiceProvider();
        }
    }
}
