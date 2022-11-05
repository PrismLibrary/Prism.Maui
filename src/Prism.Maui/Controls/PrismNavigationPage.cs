using Prism.Common;
using Prism.Navigation;

namespace Prism.Controls;

public class PrismNavigationPage : NavigationPage
{
    public PrismNavigationPage()
    {
        BackButtonPressed += HandleBackButtonPressed;
    }

    public PrismNavigationPage(Page page)
        : base(page)
    {
        BackButtonPressed += HandleBackButtonPressed;
    }

    public event EventHandler BackButtonPressed;

    protected override bool OnBackButtonPressed()
    {
        BackButtonPressed.Invoke(this, EventArgs.Empty);
        return false;
    }

    private async void HandleBackButtonPressed(object sender, EventArgs args)
    {
        bool backingOut = false;

        try
        {
            var result = await MvvmHelpers.HandleNavigationPageGoBack(this).ConfigureAwait(false);
        }
        catch (NavigationException ex)
        {
            backingOut = ex.Message == NavigationException.CannotPopApplicationMainPage;
        }

        if (backingOut)
        {
            Application.Current.Quit();
        }
    }
}