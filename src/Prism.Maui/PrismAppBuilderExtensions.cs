using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Prism.Ioc;
using Prism.Modularity;

namespace Prism
{
    public static class PrismAppBuilderExtensions
    {
        private static bool s_didRegisterModules = false;

        public static PrismAppBuilder UsePrismApp<TApp>(this MauiAppBuilder builder, IContainerExtension containerExtension)
            where TApp : PrismApplication
        {
            return new PrismAppBuilder<TApp>(containerExtension, builder);
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
            if (!s_didRegisterModules)
            {
                var services = builder.MauiBuilder.Services;
                services.AddSingleton<IModuleCatalog, ModuleCatalog>();
                services.AddSingleton<IModuleManager, ModuleManager>();
                services.AddSingleton<IModuleInitializer, ModuleInitializer>();
                services.AddSingleton<IMauiInitializeService, PrismModularityInitializationService>();
            }

            s_didRegisterModules = true;
            return builder.OnInitialized(container =>
            {
                var moduleCatalog = container.Resolve<IModuleCatalog>();
                configureCatalog(moduleCatalog);
            });
        }
    }
}
