namespace Prism.Navigation.Builder;

internal class TabbedSegmentBuilder : ITabbedSegmentBuilder, IConfigurableSegmentName, IUriSegment
{
    private INavigationParameters _parameters { get; }

    public TabbedSegmentBuilder()
    {
        _parameters = new NavigationParameters();

        var registrationInfo = NavigationRegistry.Registrations
            .FirstOrDefault(x => x.View.IsAssignableFrom(typeof(TabbedPage)));
        if (registrationInfo is null)
            throw new NavigationException(NavigationException.NoPageIsRegistered);

        SegmentName = registrationInfo.Name;
    }

    public string SegmentName { get; set; }

    public string Segment => BuildSegment();

    public ITabbedSegmentBuilder AddSegmentParameter(string key, object value)
    {
        _parameters.Add(key, value);
        return this;
    }

    public ITabbedSegmentBuilder UseModalNavigation(bool useModalNavigation)
    {
        return AddSegmentParameter(KnownNavigationParameters.UseModalNavigation, useModalNavigation);
    }

    public ITabbedSegmentBuilder CreateTab(Action<ICreateTabBuilder> configureSegment)
    {
        if (configureSegment is null)
        {
            throw new ArgumentNullException(nameof(configureSegment));
        }

        var builder = new CreateTabBuilder();
        configureSegment(builder);
        return AddSegmentParameter(KnownNavigationParameters.CreateTab, builder.Segment);
    }

    public ITabbedSegmentBuilder SelectedTab(string segmentName)
    {
        return AddSegmentParameter(KnownNavigationParameters.SelectedTab, segmentName);
    }

    private string BuildSegment()
    {
        if (!_parameters.Any())
            return SegmentName;

        return SegmentName + _parameters.ToString();
    }
}