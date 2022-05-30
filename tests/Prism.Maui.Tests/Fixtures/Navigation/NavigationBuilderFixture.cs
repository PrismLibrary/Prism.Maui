using Moq;

namespace Prism.Maui.Tests.Fixtures.Navigation;

public class NavigationBuilderFixture
{
    [Fact]
    public void GeneratesRelativeUriWithSingleSegment()
    {
        var uri = Mock.Of<INavigationService>()
            .CreateBuilder()
            .AddNavigationSegment("ViewA")
            .Uri;

        Assert.Equal("ViewA", uri.ToString());
    }

    [Fact]
    public void GeneratesRelativeUriWithMultipleSegments()
    {
        var uri = Mock.Of<INavigationService>()
            .CreateBuilder()
            .AddNavigationSegment("ViewA")
            .AddNavigationSegment("ViewB")
            .AddNavigationSegment("ViewC")
            .Uri;

        Assert.Equal("ViewA/ViewB/ViewC", uri.ToString());
    }

    [Fact]
    public void GeneratesAbsoluteUriWithSingleSegment()
    {
        var uri = Mock.Of<INavigationService>()
            .CreateBuilder()
            .AddNavigationSegment("ViewA")
            .UseAbsoluteNavigation()
            .Uri;

        Assert.Equal("app://prismapp.maui/ViewA", uri.ToString());
    }

    [Fact]
    public void GeneratesAbsoluteUriWithMultipleSegments()
    {
        var uri = Mock.Of<INavigationService>()
            .CreateBuilder()
            .AddNavigationSegment("ViewA")
            .AddNavigationSegment("ViewB")
            .AddNavigationSegment("ViewC")
            .UseAbsoluteNavigation()
            .Uri;

        Assert.Equal("app://prismapp.maui/ViewA/ViewB/ViewC", uri.ToString());
    }
}
