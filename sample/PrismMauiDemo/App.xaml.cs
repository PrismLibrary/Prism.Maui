using System.Runtime.CompilerServices;

namespace PrismMauiDemo;

public partial class App
{
    public App()
    {
        InitializeComponent();
    }


    protected override Window CreateWindow(IActivationState activationState)
    {
        return PrismWindow.Current;
    }
}
