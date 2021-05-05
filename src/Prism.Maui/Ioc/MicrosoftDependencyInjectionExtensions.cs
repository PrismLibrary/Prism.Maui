using System;
using Microsoft.Extensions.DependencyInjection;

namespace Prism.Ioc
{
    public static class MicrosoftDependencyInjectionExtensions
    {
        public static void Populate(this IContainerExtension container, IServiceCollection services)
        {
            if (!(container is IServiceCollectionAware sca))
                throw new InvalidOperationException("The instance of IContainerExtension does not implement IServiceCollectionAware");

            sca.Populate(services);
        }

        public static IServiceProvider CreateServiceProvider(this IContainerExtension container)
        {
            if (!(container is IServiceCollectionAware sca))
                throw new InvalidOperationException("The instance of IContainerExtension does not implement IServiceCollectionAware");

            return sca.CreateServiceProvider();
        }
    }
}
