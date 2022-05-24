using Prism.Ioc;
using Prism.Navigation;
using Application = Microsoft.Maui.Controls.Application;

namespace Prism;

public abstract class PrismApplication : Application, ILegacyPrismApplication
{
    private readonly IContainerExtension _containerExtension;

    /// <summary>
    /// The dependency injection container used to resolve objects
    /// </summary>
    protected IContainerProvider Container => _containerExtension;

    protected INavigationService NavigationService { get; private set; }

    protected PrismApplication()
    {
        _containerExtension = ContainerLocator.Current;
        RegisterTypes(_containerExtension);
        NavigationService = Container.Resolve<INavigationService>((typeof(IApplication), this));
        this.ModalPopping += PrismApplication_ModalPopping;
    }

    private async void PrismApplication_ModalPopping(object sender, ModalPoppingEventArgs e)
    {
        if (PageNavigationService.NavigationSource == PageNavigationSource.NavigationService)
            return;

        e.Cancel = true;
        var navService = Navigation.Xaml.Navigation.GetNavigationService(e.Modal);
        await navService.GoBackAsync();
    }

    void ILegacyPrismApplication.OnInitialized() => OnInitialized();

    // Provided to better support legacy apps updating from Prism.Forms
    protected virtual void OnInitialized() { }

    protected virtual void RegisterTypes(IContainerRegistry containerRegistry) { }
}
