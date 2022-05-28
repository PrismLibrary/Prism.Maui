using Microsoft.Extensions.Logging;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation;

namespace Prism;

public interface IPrismAppBuilderBootstrapper
{
    void ConfigureServices(IServiceCollection services);

    void RegisterTypes(IContainerRegistry containerRegistry);

    void ConfigureLogging(ILoggingBuilder loggingBuilder);

    object ConfigureDefaultViewModelFactory(IContainerProvider container, object view, Type viewModelType);

    void ConfigureModuleCatalog(IModuleCatalog moduleCatalog);

    void OnInitialized(IContainerProvider container);

    Task OnAppStart(IContainerProvider container, INavigationService navigationService);
}
