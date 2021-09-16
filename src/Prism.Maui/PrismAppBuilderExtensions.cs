using System;
using Microsoft.Extensions.DependencyInjection;
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

        public static PrismAppBuilder OnInitialized(this PrismAppBuilder builder, Action action)
        {
            return builder.OnInitialized(_ => action());
        }

        /// <summary>
        /// Configures the <see cref="IModuleCatalog"/> used by Prism.
        /// </summary>
        /// <param name="moduleCatalog">The ModuleCatalog to configure</param>
        public static PrismAppBuilder ConfigureModuleCatalog(this PrismAppBuilder builder, Action<IModuleCatalog> configureCatalog)
        {
            var services = builder.Builder.Services;
            services.AddSingleton<IModuleCatalog, ModuleCatalog>();
            services.AddSingleton<IModuleManager, ModuleManager>();
            services.AddSingleton<IModuleInitializer, ModuleInitializer>();
            services.AddSingleton<IMauiInitializeService, PrismModularityInitializationService>();
            return builder.OnInitialized(container =>
            {
                var moduleCatalog = container.Resolve<IModuleCatalog>();
                configureCatalog(moduleCatalog);
            });
        }
    }
}
