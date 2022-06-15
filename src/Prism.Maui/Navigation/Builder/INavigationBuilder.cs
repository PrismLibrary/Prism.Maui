namespace Prism.Navigation.Builder;

public interface INavigationBuilder
{
    Uri Uri { get; }
    INavigationBuilder AddSegment(string segmentName, Action<ISegmentBuilder> configureSegment);
    INavigationBuilder AddTabbedSegment(Action<ITabbedSegmentBuilder> configuration);
    INavigationBuilder WithParameters(INavigationParameters parameters);
    INavigationBuilder AddParameter(string key, object value);

    INavigationBuilder UseAbsoluteNavigation(bool absolute);
    INavigationBuilder UseRelativeNavigation();

    Task<INavigationResult> NavigateAsync();
    Task NavigateAsync(Action<Exception> onError);
    Task NavigateAsync(Action onSuccess, Action<Exception> onError);
}
