namespace Prism.Navigation;

internal class PrismWindow : Window
{
    public const string DefaultWindowName = "__PrismRootWindow";

    public PrismWindow(string name = DefaultWindowName)
    {
        Name = name;
        ModalPopping += PrismApplication_ModalPopping;
    }

    public string Name { get; }

    private async void PrismApplication_ModalPopping(object sender, ModalPoppingEventArgs e)
    {
        if (PageNavigationService.NavigationSource == PageNavigationSource.NavigationService)
            return;

        e.Cancel = true;
        var navService = Xaml.Navigation.GetNavigationService(e.Modal);
        await navService.GoBackAsync();
    }
}
