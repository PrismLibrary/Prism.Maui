using Microsoft.Maui;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using Prism;
using Prism.Ioc;
using Prism.Navigation;

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

            this.On<Microsoft.Maui.Controls.PlatformConfiguration.Windows>()
                .SetImageDirectory("Assets");

            NavigationService.NavigateAsync("MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainPage>();
        }
    }
}