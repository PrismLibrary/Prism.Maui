using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using Prism.Navigation.Xaml;
using Prism.Xaml;

namespace Prism.Services.Xaml;

[ContentProperty(nameof(Name))]
public class ShowDialogExtension : TargetAwareExtensionBase<ICommand>, ICommand
{
    public string Name { get; set; }

    public bool IsExecuting { get; set; }

    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter) =>
        !IsExecuting;

    public void Execute(object parameter)
    {
        IsExecuting = true;
        CanExecuteChanged(this, EventArgs.Empty);

        try
        {
            var parameters = parameter.ToDialogParameters(TargetElement);
            var dialogService = Page.GetContainerProvider().Resolve<IDialogService>();
            dialogService.ShowDialog(Name, parameters, DialogClosedCallback);
        }
        catch (Exception ex)
        {
            Logger.LogWarning($"An unexpected error occurred while showing the Dialog '{Name}'.\n{ex}");
        }
    }

    private void DialogClosedCallback(IDialogResult result)
    {
        OnDialogClosed(result);

        IsExecuting = false;
        CanExecuteChanged(this, EventArgs.Empty);
    }

    protected virtual void OnDialogClosed(IDialogResult result)
    {
        if (result.Exception != null)
        {
            Logger.LogWarning($"Dialog '{Name}' closed with an error:\n{result.Exception}");
        }
    }

    protected override ICommand ProvideValue(IServiceProvider serviceProvider) =>
        this;
}
