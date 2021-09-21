using System.Threading.Tasks;
using Microsoft.Maui;
using Prism;
using Prism.Navigation;
using PrismMauiDemo.Views;

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
            var result = await NavigationService.NavigateAsync("MainPage/NavigationPage/SamplePage");
            if(!result.Success)
            {
                System.Diagnostics.Debugger.Break();
            }
        }
    }
}
