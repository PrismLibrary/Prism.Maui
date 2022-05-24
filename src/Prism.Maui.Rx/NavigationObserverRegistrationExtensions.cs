using Microsoft.Extensions.DependencyInjection;
using Prism.Ioc;

namespace Prism.Navigation;

public static class NavigationObserverRegistrationExtensions
{
    private static bool s_IsRegistered;

    public static IContainerRegistry RegisterGlobalNavigationObserver(this IContainerRegistry container)
    {
        if (s_IsRegistered)
            return container;

        s_IsRegistered = true;
        return container.RegisterSingleton<IGlobalNavigationObserver, GlobalNavigationObserver>();
    }

    public static IServiceCollection RegisterGlobalNavigationObserver(this IServiceCollection services)
    {
        if (s_IsRegistered)
            return services;

        s_IsRegistered = true;
        return services.AddSingleton<IGlobalNavigationObserver, GlobalNavigationObserver>();
    }

    public static PrismAppBuilder AddGlobalNavigationObserver(this PrismAppBuilder builder, Action<IObservable<NavigationRequestContext>> addObservable) =>
        builder.OnInitialized(c =>
        {
            if (!s_IsRegistered)
                throw new Exception("IGlobalNavigationObserver has not been registered. Be sure to call 'container.RegisterGlobalNavigationObserver()'.");

            addObservable(c.Resolve<IGlobalNavigationObserver>().NavigationRequest);
        });

    public static PrismAppBuilder AddGlobalNavigationObserver(this PrismAppBuilder builder, Action<IContainerProvider, IObservable<NavigationRequestContext>> addObservable) =>
        builder.OnInitialized(c =>
        {
            if (!s_IsRegistered)
                throw new Exception("IGlobalNavigationObserver has not been registered. Be sure to call 'container.RegisterGlobalNavigationObserver()'.");

            addObservable(c, c.Resolve<IGlobalNavigationObserver>().NavigationRequest);
        });
}