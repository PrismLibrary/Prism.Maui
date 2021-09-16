using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;

namespace Prism.Modularity
{
    internal class PrismModularityInitializationService : IMauiInitializeService
    {
        /// <summary>
        /// Initializes the modules.
        /// </summary>
        public void Initialize(IServiceProvider services)
        {
            var moduleCatalog = services.GetRequiredService<IModuleCatalog>();
            if (moduleCatalog.Modules.Any())
            {
                var manager = services.GetRequiredService<IModuleManager>();
                manager.Run();
            }
        }
    }
}
