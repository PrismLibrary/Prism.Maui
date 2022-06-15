namespace Prism.Navigation;

public record NavigationResult : INavigationResult
{
    public Exception Exception { get; init; }
}
