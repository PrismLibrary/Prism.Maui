using System;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Prism.Ioc;

namespace Prism.DryIoc
{
    partial class DryIocContainerExtension : IServiceCollectionAware
    {
        public IServiceProvider CreateServiceProvider()
        {
            return Instance.BuildServiceProvider();
        }

        public void Populate(IServiceCollection services)
        {
            Instance.Populate(services);
        }
    }
}
