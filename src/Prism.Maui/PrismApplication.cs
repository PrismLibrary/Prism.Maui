using Prism.Extensions;
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
        NavigationService = Container.Resolve<INavigationService>((typeof(IApplication), this));
    }

    void ILegacyPrismApplication.OnInitialized() => OnInitialized();

    // Provided to better support legacy apps updating from Prism.Forms
    protected virtual void OnInitialized() { }

    protected sealed override Window CreateWindow(IActivationState activationState) =>
        this.GetDefaultPrismWindow();
}
