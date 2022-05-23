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
}