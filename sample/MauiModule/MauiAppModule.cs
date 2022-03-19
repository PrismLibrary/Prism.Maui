using MauiModule.ViewModels;
using MauiModule.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace MauiModule
{
    // All the code in this file is included in all platforms.
    public class MauiAppModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ViewA, ViewAViewModel>();
            containerRegistry.RegisterForNavigation<ViewB, ViewBViewModel>();
            containerRegistry.RegisterForNavigation<ViewC, ViewCViewModel>();
            containerRegistry.RegisterForNavigation<ViewD, ViewDViewModel>();
        }
    }
}