namespace Prism.DryIoc.Maui.Tests.Mocks;

public class MockHome : FlyoutPage
{
    public MockHome()
    {
        Flyout = new ContentPage { Title = "Menu" };
    }
}
