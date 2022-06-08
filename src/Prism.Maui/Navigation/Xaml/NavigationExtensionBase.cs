using Prism.Xaml;
using System.Windows.Input;

namespace Prism.Navigation.Xaml;

public abstract class NavigationExtensionBase : Prism.Xaml.TargetAwareExtensionBase<ICommand>, ICommand
{
    public static readonly BindableProperty AnimatedProperty =
        BindableProperty.Create(nameof(Animated), typeof(bool), typeof(NavigationExtensionBase), true);

    public static readonly BindableProperty UseModalNavigationProperty =
        BindableProperty.Create(nameof(UseModalNavigation), typeof(bool?), typeof(NavigationExtensionBase), null);

    protected internal bool IsNavigating { get; private set; }

    public bool Animated
    {
        get => (bool)GetValue(AnimatedProperty);
        set => SetValue(AnimatedProperty, value);
    }

    public bool? UseModalNavigation
    {
        get => (bool?)GetValue(UseModalNavigationProperty);
        set => SetValue(UseModalNavigationProperty, value);
    }

    public bool CanExecute(object parameter) => !IsNavigating;

    public event EventHandler CanExecuteChanged;

    public async void Execute(object parameter)
    {
        var parameters = parameter.ToNavigationParameters(TargetElement);

        IsNavigating = true;
        try
        {
            RaiseCanExecuteChanged();

            var navigationService = Navigation.GetNavigationService(Page);
            await HandleNavigation(parameters, navigationService);
        }
        catch (Exception ex)
        {
            Log(ex, parameters);
        }
        finally
        {
            IsNavigating = false;
            RaiseCanExecuteChanged();
        }
    }

    protected override ICommand ProvideValue(IServiceProvider serviceProvider) =>
        this;

    protected virtual void Log(Exception ex, INavigationParameters parameters)
    {
        //TODO: What to do with logs?
        //Xamarin.Forms.Internals.Log.Warning("Warning", $"{GetType().Name} threw an exception");
        //Xamarin.Forms.Internals.Log.Warning("Exception", ex.ToString());
    }

    protected abstract Task HandleNavigation(INavigationParameters parameters, INavigationService navigationService);

    protected void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    protected void AddKnownNavigationParameters(INavigationParameters parameters)
    {
        parameters.Add(KnownNavigationParameters.Animated, Animated);
        parameters.Add(KnownNavigationParameters.UseModalNavigation, UseModalNavigation);
    }
}