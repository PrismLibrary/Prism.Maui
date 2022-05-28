using Microsoft.Extensions.Logging;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation;

namespace Prism;

public abstract class PrismBootstrapper : IPrismAppBuilderBootstrapper
{
    public virtual object ConfigureDefaultViewModelFactory(IContainerProvider container, object view, Type viewModelType)
    {
        return PrismAppBuilder.DefaultViewModelLocator(view, viewModelType);
    }

    public virtual void ConfigureLogging(ILoggingBuilder loggingBuilder)
    {
    }

    public virtual void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
    }

    public virtual void ConfigureServices(IServiceCollection services)
    {
    }

    public abstract Task OnAppStart(IContainerProvider container, INavigationService navigationService);

    public virtual void OnInitialized(IContainerProvider container)
    {
    }

    public virtual void RegisterTypes(IContainerRegistry containerRegistry)
    {
    }
}
