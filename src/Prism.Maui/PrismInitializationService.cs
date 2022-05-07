using Prism.Modularity;

namespace Prism;

internal class PrismInitializationService : IMauiInitializeService
{
    /// <summary>
    /// Initializes the modules.
    /// </summary>
    public void Initialize(IServiceProvider services)
    {
        var moduleCatalog = services.GetService<IModuleCatalog>();
        if (moduleCatalog is not null && moduleCatalog.Modules.Any())
        {
            var manager = services.GetRequiredService<IModuleManager>();
            manager.Run();
        }

        var app = services.GetRequiredService<IApplication>();
        if (app is IPrismApplication prismApp)
            prismApp.OnInitialized();
    }
}
