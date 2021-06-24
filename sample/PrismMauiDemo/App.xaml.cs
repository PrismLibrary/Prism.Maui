using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using Prism;
using Prism.Ioc;
using Prism.Navigation;

namespace PrismMauiDemo
{
    public partial class App : PrismApplication
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Task OnWindowCreated(IActivationState activationState)
        {
            this.On<Microsoft.Maui.Controls.PlatformConfiguration.Windows>()
                .SetImageDirectory("Assets");

            return NavigationService.NavigateAsync("MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainPage>();
        }
    }
}
