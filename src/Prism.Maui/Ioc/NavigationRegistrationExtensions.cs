using Microsoft.Maui.Controls;
using Prism.Navigation;

namespace Prism.Ioc
{
    public static class NavigationRegistrationExtensions
    {
        public static IContainerRegistry RegisterForNavigation<TView>(this IContainerRegistry container, string name = null)
            where TView : Page =>
            container.RegisterForNavigation(typeof(TView), null, name);

        public static IContainerRegistry RegisterForNavigation<TView, TViewModel>(this IContainerRegistry container, string name = null)
            where TView : Page =>
            container.Register(typeof(TView), typeof(TViewModel), name);

        public static IContainerRegistry RegisterForNavigation(this IContainerRegistry container, Type view, Type viewModel, string name = null)
        {
            if (view is null)
                throw new ArgumentNullException(nameof(view));

            if (string.IsNullOrEmpty(name))
                name = view.Name;

            NavigationRegistry.Register(view, viewModel, name);
            container.Register(view);

            if (viewModel != null)
                container.Register(viewModel);
            return container;
        }
    }
}
