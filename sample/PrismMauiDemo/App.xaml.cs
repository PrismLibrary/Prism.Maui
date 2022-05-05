using Prism;
using Prism.Navigation;

namespace PrismMauiDemo;

public partial class App : PrismApplication
{
    public App()
    {
        InitializeComponent();
    }

    protected override async Task OnWindowCreated(IActivationState activationState)
    {
        var result = await NavigationService.NavigateAsync("MainPage/NavigationPage/ViewA");
        if (!result.Success)
        {
            System.Diagnostics.Debugger.Break();
        }
    }
}
