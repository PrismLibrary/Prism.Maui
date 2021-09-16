using System.Threading.Tasks;
using Microsoft.Maui;
using Prism;
using Prism.Navigation;

namespace PrismMauiDemo
{
    public partial class App : PrismApplication
    {
        public App()
        {
            InitializeComponent();
        }

        protected override async Task OnWindowCreated(IActivationState activationState)
        {
            await NavigationService.NavigateAsync("MainPage");
        }
    }
}
