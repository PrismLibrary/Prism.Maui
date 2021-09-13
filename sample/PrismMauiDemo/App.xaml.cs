using Microsoft.Maui;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Hosting;
using Prism;
using Prism.Ioc;
using Prism.Navigation;

[assembly: XamlCompilationAttribute(XamlCompilationOptions.Compile)]
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
            return NavigationService.NavigateAsync("MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainPage>();
        }

        protected override void Configure(IAppHostBuilder builder)
        {
            builder
               .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
        }
    }
}
