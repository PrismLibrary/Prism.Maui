using Moq;

namespace Prism.Maui.Tests.Fixtures.Navigation;

public class NavigationBuilderFixture
{
    [Fact]
    public void GeneratesRelativeUriWithSingleSegment()
    {
        var uri = Mock.Of<INavigationService>()
            .CreateBuilder()
            .AddSegment("ViewA")
            .Uri;

        Assert.Equal("ViewA", uri.ToString());
    }

    [Fact]
    public void GeneratesRelativeUriWithMultipleSegments()
    {
        var uri = Mock.Of<INavigationService>()
            .CreateBuilder()
            .AddSegment("ViewA")
            .AddSegment("ViewB")
            .AddSegment("ViewC")
            .Uri;

        Assert.Equal("ViewA/ViewB/ViewC", uri.ToString());
    }

    [Fact]
    public void GeneratesAbsoluteUriWithSingleSegment()
    {
        var uri = Mock.Of<INavigationService>()
            .CreateBuilder()
            .AddSegment("ViewA")
            .UseAbsoluteNavigation()
            .Uri;

        Assert.Equal("app://prismapp.maui/ViewA", uri.ToString());
    }

    [Fact]
    public void GeneratesAbsoluteUriWithMultipleSegments()
    {
        var uri = Mock.Of<INavigationService>()
            .CreateBuilder()
            .AddSegment("ViewA")
            .AddSegment("ViewB")
            .AddSegment("ViewC")
            .UseAbsoluteNavigation()
            .Uri;

        Assert.Equal("app://prismapp.maui/ViewA/ViewB/ViewC", uri.ToString());
    }
}
