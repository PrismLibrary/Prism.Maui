using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Prism.Ioc
{
    public class PrismServiceProviderFactory : IServiceProviderFactory<IContainerExtension>
    {
        private List<Action<IContainerProvider>> _initializations { get; }

        public PrismServiceProviderFactory(List<Action<IContainerProvider>> initializations)
        {
            _initializations = initializations;
        }

        public IContainerExtension CreateBuilder(IServiceCollection services)
        {
            var container = ContainerLocator.Current;
            container.Populate(services);
            _initializations.ForEach(action => action(container));
            return container;
        }

        public IServiceProvider CreateServiceProvider(IContainerExtension containerExtension)
        {
            return containerExtension.CreateServiceProvider();
        }
    }
}
