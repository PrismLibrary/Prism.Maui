namespace Prism.Services.Dialogs;

public record DialogResult : IDialogResult
{
    public Exception Exception { get; init; }
    public IDialogParameters Parameters { get; init; }
}
