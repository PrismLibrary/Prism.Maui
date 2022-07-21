namespace Prism.Services;

public static class IDialogServiceExtensions
{
    public static void ShowDialog(this IDialogService dialogService, string name, IDialogParameters parameters) =>
        dialogService.ShowDialog(name, parameters, _ => Task.CompletedTask);

    public static void ShowDialog(this IDialogService dialogService, string name) =>
        dialogService.ShowDialog(name, new DialogParameters());

    public static Task<IDialogResult> ShowDialogAsync(this IDialogService dialogService, string name) =>
        dialogService.ShowDialogAsync(name, new DialogParameters());

    public static Task<IDialogResult> ShowDialogAsync(this IDialogService dialogService, string name, IDialogParameters parameters)
    {
        var tcs = new TaskCompletionSource<IDialogResult>();
        dialogService.ShowDialog(name, parameters, result => tcs.SetResult(result));
        return tcs.Task;
    }
}
