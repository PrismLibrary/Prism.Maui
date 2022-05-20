namespace Prism.Navigation.Builder;

internal class NavigationBuilder : INavigationBuilder
{
    internal static readonly Uri RootUri = new Uri("app://prismapp.maui", UriKind.Absolute);

    private INavigationService _navigationService { get; }
    private INavigationParameters _navigationParameters { get; }
    private bool _absoluteNavigation;
    private List<IUriSegment> _uriSegments { get; }

    public NavigationBuilder(INavigationService navigationService)
    {
        _navigationService = navigationService;
        _navigationParameters = new NavigationParameters();
        _uriSegments = new List<IUriSegment>();
    }

    public INavigationBuilder AddNavigationSegment(string segmentName, Action<ISegmentBuilder> configureSegment)
    {
        var builder = new SegmentBuilder(segmentName);
        configureSegment?.Invoke(builder);
        _uriSegments.Add(builder);
        return this;
    }

    public INavigationBuilder AddTabbedSegment(Action<ITabbedSegmentBuilder> configureSegment)
    {
        var builder = new TabbedSegmentBuilder();
        configureSegment?.Invoke(builder);
        _uriSegments.Add(builder);
        return this;
    }

    public INavigationBuilder AddParameter(string key, object value)
    {
        _navigationParameters.Add(key, value);
        return this;
    }

    public Task<INavigationResult> NavigateAsync()
    {
        return _navigationService.NavigateAsync(BuildUri(), _navigationParameters);
    }

    public Task NavigateAsync(Action<Exception> onError)
    {
        return NavigateAsync(() => { }, onError);
    }

    public async Task NavigateAsync(Action onSuccess, Action<Exception> onError)
    {
        var result = await NavigateAsync();
        if (result.Exception != null)
            onError?.Invoke(result.Exception);
        else if (result.Success)
            onSuccess?.Invoke();
    }

    public INavigationBuilder UseAbsoluteNavigation(bool absolute)
    {
        _absoluteNavigation = absolute;
        return this;
    }

    public INavigationBuilder UseRelativeNavigation()
    {
        _absoluteNavigation = false;
        return this;
    }

    public INavigationBuilder WithParameters(INavigationParameters parameters)
    {
        foreach ((var key, var value) in parameters)
            _navigationParameters.Add(key, value);

        return this;
    }

    internal Uri BuildUri()
    {
        var uri = string.Join("/", _uriSegments.Select(x => x.Segment));

        return _absoluteNavigation ? new Uri(RootUri, uri) : new Uri(uri, UriKind.Relative);
    }
}
