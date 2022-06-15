using System.Web;

namespace Prism.Navigation.Builder;

internal class CreateTabBuilder : ICreateTabBuilder, IUriSegment
{
    private List<IUriSegment> _segments { get; }

    public CreateTabBuilder()
    {
        _segments = new List<IUriSegment>();
    }

    public string Segment => BuildSegment();

    public ICreateTabBuilder AddSegment(string segmentName, Action<ISegmentBuilder> configureSegment)
    {
        var builder = new SegmentBuilder(segmentName);
        configureSegment?.Invoke(builder);
        _segments.Add(builder);
        return this;
    }

    private string BuildSegment()
    {
        var uri = string.Join("/", _segments.Select(x => x.Segment));
        return HttpUtility.UrlEncode(uri);
    }
}
