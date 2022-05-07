using Prism;
using Prism.Navigation;

namespace PrismMauiDemo;

public partial class App : PrismApplication
{
    public App()
    {
        InitializeComponent();
    }

    // Provided mainly for Legacy Support...
    // This can be done via the builder and a standard Maui Application
    protected override void OnInitialized()
    {
        //var result = await NavigationService.NavigateAsync("MainPage/NavigationPage/ViewA");
        //if (!result.Success)
        //{
        //    System.Diagnostics.Debugger.Break();
        //}
    }
}
