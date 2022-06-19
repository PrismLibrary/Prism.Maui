using Prism.Behaviors;
using Prism.Extensions;
using Prism.Ioc;
using Prism.Navigation.Xaml;
using Prism.Properties;

namespace Prism.Xaml;

public abstract class TargetAwareExtensionBase<T> : BindableObject, IMarkupExtension<T>
{
    private Page _page;
    protected internal Page Page
    {
        get => _page;
        set
        {
            OnPropertyChanging(nameof(Page));
            _page = value;
            OnPropertyChanged(nameof(Page));
        }
    }

    private VisualElement _targetElement;
    protected internal VisualElement TargetElement
    {
        get => _targetElement;
        set
        {
            OnPropertyChanging(nameof(TargetElement));
            _targetElement = value;
            OnPropertyChanged(nameof(TargetElement));
        }
    }

    protected IContainerProvider Container => TargetElement.GetContainerProvider();

    /// <summary>
    /// Sets the Target BindingContext strategy
    /// </summary>
    public TargetBindingContext TargetBindingContext { get; set; }

    T IMarkupExtension<T>.ProvideValue(IServiceProvider serviceProvider)
    {
        var valueTargetProvider = serviceProvider.GetService<IProvideValueTarget>();

        if (valueTargetProvider == null)
            throw new ArgumentException(Resources.ServiceProviderDidNotHaveIProvideValueTarget);

        TargetElement = valueTargetProvider.TargetObject as VisualElement;

        //this is handling the scenario of the extension being used within the EventToCommandBehavior
        if (TargetElement is null && valueTargetProvider.TargetObject is BehaviorBase<BindableObject> behavior)
            TargetElement = behavior.AssociatedObject as VisualElement;

        if (TargetElement is null)
            throw new Exception($"{valueTargetProvider.TargetObject} is not supported");

        var path = TargetBindingContext switch
        {
            TargetBindingContext.Element => "TargetElement.BindingContext",
            _ => "Page.BindingContext"
        };

        SetBinding(BindingContextProperty, new Binding(path, BindingMode.OneWay, source: this));

        if (TargetElement.TryGetParentPage(out var page))
            Page = page;
        else
            TargetElement.Behaviors.Add(new ElementParentedCallbackBehavior(() => Page = TargetElement.GetParentPage()));

        return ProvideValue(serviceProvider);
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => 
        ((IMarkupExtension<T>)this).ProvideValue(serviceProvider);

    protected abstract T ProvideValue(IServiceProvider serviceProvider);
}
