namespace Prism.Navigation.Builder;

internal class SegmentBuilder : ISegmentBuilder, IConfigurableSegmentName, IUriSegment
{
    private INavigationParameters _parameters { get; }

    public SegmentBuilder(string segmentName)
    {
        _parameters = new NavigationParameters();
        SegmentName = segmentName;
    }

    public string SegmentName { get; set; }

    public string Segment => BuildSegment();

    public ISegmentBuilder AddSegmentParameter(string key, object value)
    {
        _parameters.Add(key, value);
        return this;
    }

    public ISegmentBuilder UseModalNavigation(bool useModalNavigation)
    {
        return AddSegmentParameter(KnownNavigationParameters.UseModalNavigation, useModalNavigation);
    }

    private string BuildSegment()
    {
        if (!_parameters.Any())
            return SegmentName;

        return SegmentName + _parameters.ToString();
    }
}
