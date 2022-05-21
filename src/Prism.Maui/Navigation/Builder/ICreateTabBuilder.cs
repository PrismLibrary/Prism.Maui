namespace Prism.Navigation.Builder;

public interface ICreateTabBuilder
{
    ICreateTabBuilder AddNavigationSegment(string segmentName, Action<ISegmentBuilder> configureSegment);
}
