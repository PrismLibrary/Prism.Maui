namespace Prism.Navigation.Builder;

public interface ISegmentBuilder
{
    string SegmentName { get; }

    ISegmentBuilder AddSegmentParameter(string key, object value);

    ISegmentBuilder UseModalNavigation(bool useModalNavigation);
}
