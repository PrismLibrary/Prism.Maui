﻿using System;
using Prism.Common;

namespace Prism.Maui.Tests.Fixtures.Common;

public class UriParsingHelperFixture
{
    const string _relativeUri = "MainPage?id=3&name=dan";
    const string _absoluteUriWithOutProtocol = "/MainPage?id=3&name=dan";
    const string _absoluteUri = "htp://www.dansiegel.net/MainPage?id=3&name=dan";
    const string _deepLinkAbsoluteUri = "android-app://HellowWorld/MainPage?id=1/ViewA?id=2/ViewB?id=3/ViewC?id=4";
    const string _deepLinkRelativeUri = "MainPage?id=1/ViewA?id=2/ViewB?id=3/ViewC?id=4";

    [Fact]
    public void ParametersParsedFromNullSegment()
    {
        var parameters = UriParsingHelper.GetSegmentParameters(null);
        Assert.NotNull(parameters);
    }

    [Fact]
    public void ParametersParsedFromEmptySegment()
    {
        var parameters = UriParsingHelper.GetSegmentParameters(string.Empty);
        Assert.NotNull(parameters);
    }

    [Fact]
    public void ParametersParsedFromRelativeUri()
    {
        var parameters = UriParsingHelper.GetSegmentParameters(_relativeUri);

        Assert.NotEmpty(parameters);

        Assert.Contains("id", parameters.Keys);
        Assert.Contains("name", parameters.Keys);

        Assert.Equal("3", parameters["id"]);
        Assert.Equal("dan", parameters["name"]);
    }

    [Fact]
    public void ParametersParsedFromAbsoluteUri()
    {
        var parameters = UriParsingHelper.GetSegmentParameters(_absoluteUri);

        Assert.NotEmpty(parameters);

        Assert.Contains("id", parameters.Keys);
        Assert.Contains("name", parameters.Keys);

        Assert.Equal("3", parameters["id"]);
        Assert.Equal("dan", parameters["name"]);
    }

    [Fact]
    public void ParametersParsedFromNavigationParametersInRelativeUri()
    {
        var navParameters = new NavigationParameters
            {
                { "id", 3 },
                { "name", "dan" }
            };

        var parameters = UriParsingHelper.GetSegmentParameters("MainPage" + navParameters.ToString());

        Assert.NotEmpty(parameters);

        Assert.Contains("id", parameters.Keys);
        Assert.Contains("name", parameters.Keys);

        Assert.Equal("3", parameters["id"]);
        Assert.Equal("dan", parameters["name"]);
    }

    [Fact]
    public void ParametersParsedFromNavigationParametersInAbsoluteUri()
    {
        var navParameters = new NavigationParameters
            {
                { "id", 3 },
                { "name", "dan" }
            };

        var parameters = UriParsingHelper.GetSegmentParameters("http://www.dansiegel.net/MainPage" + navParameters.ToString());

        Assert.NotEmpty(parameters);

        Assert.Contains("id", parameters.Keys);
        Assert.Contains("name", parameters.Keys);

        Assert.Equal("3", parameters["id"]);
        Assert.Equal("dan", parameters["name"]);
    }

    [Fact]
    public void TargetNameParsedFromSingleSegment()
    {
        var target = UriParsingHelper.GetSegmentName(_relativeUri);
        Assert.Equal("MainPage", target);
    }

    [Fact]
    public void SegmentsParsedFromDeepLinkUri()
    {
        var target = UriParsingHelper.GetUriSegments(new Uri(_deepLinkAbsoluteUri));
        Assert.Equal(4, target.Count);
    }

    [Fact]
    public void ParametersParsedFromDeepLinkAbsoluteUri()
    {
        var target = UriParsingHelper.GetUriSegments(new Uri(_deepLinkAbsoluteUri));
        Assert.Equal(4, target.Count);

        var p1 = UriParsingHelper.GetSegmentParameters(target.Dequeue());
        Assert.Equal("1", p1["id"]);

        var p2 = UriParsingHelper.GetSegmentParameters(target.Dequeue());
        Assert.Equal("2", p2["id"]);

        var p3 = UriParsingHelper.GetSegmentParameters(target.Dequeue());
        Assert.Equal("3", p3["id"]);

        var p4 = UriParsingHelper.GetSegmentParameters(target.Dequeue());
        Assert.Equal("4", p4["id"]);
    }

    [Fact]
    public void ParametersParsedFromDeepLinkRelativeUri()
    {
        var target = UriParsingHelper.GetUriSegments(new Uri(_deepLinkRelativeUri, UriKind.Relative));
        Assert.Equal(4, target.Count);

        var p1 = UriParsingHelper.GetSegmentParameters(target.Dequeue());
        Assert.Equal("1", p1["id"]);

        var p2 = UriParsingHelper.GetSegmentParameters(target.Dequeue());
        Assert.Equal("2", p2["id"]);

        var p3 = UriParsingHelper.GetSegmentParameters(target.Dequeue());
        Assert.Equal("3", p3["id"]);

        var p4 = UriParsingHelper.GetSegmentParameters(target.Dequeue());
        Assert.Equal("4", p4["id"]);
    }

    [Fact]
    public void ParametersParsedFromUriWithEmptyPathSegments()
    {
        var uri = new Uri("app://forms/MainPage//DetailPage");
        var target = UriParsingHelper.GetUriSegments(uri);
        Assert.Equal(2, target.Count);
    }

    [Fact]
    public void EnsureAbsoluteUriForRelativeUri()
    {
        var uri = UriParsingHelper.EnsureAbsolute(new Uri(_relativeUri, UriKind.Relative));
        Assert.True(uri.IsAbsoluteUri);
    }

    [Fact]
    public void EnsureAbsoluteUriForRelativeUriThatStartsWithSlash()
    {
        var uri = UriParsingHelper.EnsureAbsolute(new Uri("/" + _relativeUri, UriKind.Relative));
        Assert.True(uri.IsAbsoluteUri);
    }

    [Fact]
    public void EnsureAbsoluteUriForAbsoluteUri()
    {
        var uri = UriParsingHelper.EnsureAbsolute(new Uri(_absoluteUri, UriKind.Absolute));
        Assert.True(uri.IsAbsoluteUri);
    }

    [Fact]
    public void ParseForNull()
    {
        var actual = Assert.Throws<ArgumentNullException>(() => UriParsingHelper.Parse(null));
        Assert.NotNull(actual);
        Assert.Equal("uri", actual.ParamName);
    }

    [Fact]
    public void ParseForRelativeUri()
    {
        var uri = UriParsingHelper.Parse(_relativeUri);
        Assert.NotNull(uri);
        Assert.Equal(_relativeUri, uri.OriginalString);
        Assert.False(uri.IsAbsoluteUri);
    }

    [Fact]
    public void ParseForAbsoluteUri()
    {
        var uri = UriParsingHelper.Parse(_absoluteUri);
        Assert.NotNull(uri);
        Assert.Equal(_absoluteUri, uri.OriginalString);
        Assert.True(uri.IsAbsoluteUri);
    }

    [Fact]
    public void ParseForAbsoluteUriWithOutProtocol()
    {
        var uri = UriParsingHelper.Parse(_absoluteUriWithOutProtocol);
        Assert.NotNull(uri);
        Assert.Equal("app://prismapp.maui" + _absoluteUriWithOutProtocol, uri.OriginalString);
        Assert.True(uri.IsAbsoluteUri);
    }

    [Fact]
    public void ParseForDeepLinkAbsoluteUri()
    {
        var uri = UriParsingHelper.Parse(_deepLinkAbsoluteUri);
        Assert.NotNull(uri);
        Assert.Equal(_deepLinkAbsoluteUri, uri.OriginalString);
        Assert.True(uri.IsAbsoluteUri);
    }

    [Fact]
    public void ParseForDeepLinkRelativeUri()
    {
        var uri = UriParsingHelper.Parse(_deepLinkRelativeUri);
        Assert.NotNull(uri);
        Assert.Equal(_deepLinkRelativeUri, uri.OriginalString);
        Assert.False(uri.IsAbsoluteUri);
    }
}
