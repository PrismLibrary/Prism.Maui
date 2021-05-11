using Microsoft.Maui;
using Prism;
using Prism.Ioc;

namespace PrismMauiDemo
{
    public partial class App : PrismApplication
    {
        public App(IContainerExtension container)
            : base(container)
        {
        }

        protected override void OnWindowCreated(IActivationState activationState)
        {
            Microsoft.Maui.Controls.Compatibility.Forms.Init(activationState);
            NavigationService.NavigateAsync("MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainPage>();
        }
    }
}