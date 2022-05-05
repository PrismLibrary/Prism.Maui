namespace Prism.Mvvm;

/// <summary>
/// This class defines the attached property and related change handler that calls the <see cref="Prism.Mvvm.ViewModelLocationProvider2"/>.
/// </summary>
public static class ViewModelLocator
{
    /// <summary>
    /// Instructs Prism whether or not to automatically create an instance of a ViewModel using a convention, and assign the associated View's <see cref="BindableObject.BindingContext"/> to that instance.
    /// </summary>
    public static readonly BindableProperty AutowireViewModelProperty =
        BindableProperty.CreateAttached("AutowireViewModel", typeof(bool?), typeof(ViewModelLocator), null, propertyChanged: OnAutowireViewModelChanged);

    internal static readonly BindableProperty ViewModelProperty =
        BindableProperty.CreateAttached("ViewModelType",
            typeof(Type),
            typeof(ViewModelLocator),
            null,
            propertyChanged: OnViewModelPropertyChanged);

    /// <summary>
    /// Gets the AutowireViewModel property value.
    /// </summary>
    /// <param name="bindable"></param>
    /// <returns></returns>
    public static bool? GetAutowireViewModel(BindableObject bindable)
    {
        return (bool?)bindable.GetValue(AutowireViewModelProperty);
    }

    /// <summary>
    /// Sets the AutowireViewModel property value.  If <c>true</c>, creates an instance of a ViewModel using a convention, and sets the associated View's <see cref="BindableObject.BindingContext"/> to that instance.
    /// </summary>
    /// <param name="bindable"></param>
    /// <param name="value"></param>
    public static void SetAutowireViewModel(BindableObject bindable, bool? value)
    {
        bindable.SetValue(AutowireViewModelProperty, value);
    }

    private static void OnAutowireViewModelChanged(BindableObject bindable, object oldValue, object newValue)
    {
        bool? bNewValue = (bool?)newValue;
        if (bNewValue.HasValue && bNewValue.Value)
        {
            var vmType = bindable.GetValue(ViewModelProperty) as Type;
            if (vmType is null)
                ViewModelLocationProvider2.AutoWireViewModelChanged(bindable, Bind);
        }
    }

    private static void OnViewModelPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (newValue == null || bindable.BindingContext != null)
            return;
        else if(newValue is Type)
            bindable.SetValue(AutowireViewModelProperty, true);
    }

    /// <summary>
    /// Sets the <see cref="BindableObject.BindingContext"/> of a View
    /// </summary>
    /// <param name="view">The View to set the <see cref="BindableObject.BindingContext"/> on</param>
    /// <param name="viewModel">The object to use as the <see cref="BindableObject.BindingContext"/> for the View</param>
    private static void Bind(object view, object viewModel)
    {
        if (view is BindableObject element)
            element.BindingContext = viewModel;
    }
}
