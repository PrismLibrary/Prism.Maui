using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation;
using Microsoft.Extensions.Logging;

namespace Prism;

public static class PrismAppBuilderExtensions
{
    private static bool s_didRegisterModules = false;

    public static PrismAppBuilder UsePrismApp<TApp>(this MauiAppBuilder builder, IContainerExtension containerExtension)
        where TApp : Application
    {
        return new PrismAppBuilder<TApp>(containerExtension, builder);
    }

    public static MauiAppBuilder ConfigureWithBootstrapper<T>(this PrismAppBuilder builder)
        where T : IPrismAppBuilderBootstrapper, new()
    {
        var bootstrapper = new T();
        builder.ConfigureServices(bootstrapper.ConfigureServices)
            .ConfigureLogging(bootstrapper.ConfigureLogging)
            .ConfigureDefaultViewModelFactory(bootstrapper.ConfigureDefaultViewModelFactory)
            .RegisterTypes(bootstrapper.RegisterTypes)
            .ConfigureModuleCatalog(bootstrapper.ConfigureModuleCatalog)
            .OnInitialized(bootstrapper.OnInitialized)
            .OnAppStart(bootstrapper.OnAppStart);

        return builder.MauiBuilder;
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
        }

        s_didRegisterModules = true;
        return builder.OnInitialized(container =>
        {
            var moduleCatalog = container.Resolve<IModuleCatalog>();
            configureCatalog(moduleCatalog);
        });
    }

    public static MauiAppBuilder OnAppStart(this PrismAppBuilder builder, Action<INavigationService> onAppStarted) =>
        builder.OnAppStart((_, n) => onAppStarted(n));

    public static MauiAppBuilder OnAppStart(this PrismAppBuilder builder, Func<IContainerProvider, INavigationService, Task> onAppStarted) =>
        builder.OnAppStart(async (c, n) => await onAppStarted(c, n));

    public static MauiAppBuilder OnAppStart(this PrismAppBuilder builder, Func<INavigationService, Task> onAppStarted) =>
        builder.OnAppStart(async (_, n) => await onAppStarted(n));

    public static PrismAppBuilder ConfigureServices(this PrismAppBuilder builder, Action<IServiceCollection> configureServices)
    {
        configureServices(builder.MauiBuilder.Services);
        return builder;
    }

    public static PrismAppBuilder ConfigureLogging(this PrismAppBuilder builder, Action<ILoggingBuilder> configureLogging)
    {
        configureLogging(builder.MauiBuilder.Logging);
        return builder;
    }
}
