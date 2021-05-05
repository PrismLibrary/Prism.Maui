using System;
using Microsoft.Extensions.DependencyInjection;

namespace Prism.Ioc
{
    public interface IServiceCollectionAware
    {
        void Populate(IServiceCollection services);
        IServiceProvider CreateServiceProvider();
    }
}
