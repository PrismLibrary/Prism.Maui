namespace Prism.Extensions;

internal static class VisualElementExtensions
{
    public static bool TryGetParentPage(this VisualElement element, out Page page)
    {
        page = GetParentPage(element);
        return page != null;
    }

    public static Element GetRoot(this Element element)
    {
        return element.Parent switch
        {
            null => element,
            _ => GetRoot(element.Parent),
        };
    }

    private static Page GetParentPage(Element visualElement)
    {
        return visualElement.Parent switch
        {
            Page page => page,
            null => null,
            _ => GetParentPage(visualElement.Parent),
        };
    }
}
