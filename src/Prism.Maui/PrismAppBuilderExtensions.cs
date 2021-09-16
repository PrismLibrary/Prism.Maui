using System;
using Microsoft.Maui.Hosting;
using Prism.Ioc;
using Prism.Modularity;

namespace Prism
{
    public static class PrismAppBuilderExtensions
    {
        public static PrismAppBuilder WithContainerExtension(this PrismAppBuilder builder, IContainerExtension container)
        {
            ContainerLocator.SetContainerExtension(() => container);
            return builder;
        }

        public static PrismAppBuilder WithContainerExtension(this PrismAppBuilder builder, Func<IContainerExtension> containerDelegate)
        {
            ContainerLocator.SetContainerExtension(containerDelegate);
            return builder;
        }

        /// <summary>
        /// Configures the <see cref="IModuleCatalog"/> used by Prism.
        /// </summary>
        /// <param name="moduleCatalog">The ModuleCatalog to configure</param>
        public static PrismAppBuilder ConfigureModuleCatalog(this PrismAppBuilder builder, Action<IModuleCatalog> configureCatalog)
        {
            var moduleCatalog = builder.Container.Resolve<IModuleCatalog>();
            configureCatalog(moduleCatalog);
            builder.Container.RegisterSingleton<IMauiInitializeService, PrismModularityInitializationService>();
            return builder;
        }
    }
}
