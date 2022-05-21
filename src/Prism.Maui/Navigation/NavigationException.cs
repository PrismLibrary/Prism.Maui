namespace Prism.Navigation;

public class NavigationException : Exception
{
    public const string CannotPopApplicationMainPage = "Cannot Pop Application MainPage";
    public const string CannotGoBackFromRoot = "Cannot GoBack from NavigationPage Root.";
    public const string GoBackToRootRequiresNavigationPage = "GoBackToRootAsync can only be called when the calling Page is within a NavigationPage.";
    public const string RelativeNavigationRequiresNavigationPage = "Removing views using the relative '../' syntax while navigating is only supported within a NavigationPage";
    public const string IConfirmNavigationReturnedFalse = "IConfirmNavigation returned false";
    public const string NoPageIsRegistered = "No Page has been registered with the provided key";
    public const string ErrorCreatingPage = "An error occurred while resolving the page. This is most likely the result of invalid XAML or other type initialization exception";
    public const string MvvmPatternBreak = "You have referenced a View type and are likely breaking the MVVM pattern. You should never reference a View type from a ViewModel.";
    public const string UnknownException = "An unknown error occurred. You may need to specify whether to Use Modal Navigation or not.";

    public NavigationException()
    {
    }

    public NavigationException(string message)
        : this(message, null, null, null)
    {
    }

    public NavigationException(string message, Page page)
        : this(message, page, null)
    {
    }

    public NavigationException(string message, string navigationKey)
        : this(message, navigationKey, null, null)
    {
    }

    public NavigationException(string message, string navigationKey, Exception innerException)
        : this(message, navigationKey, null, innerException)
    {
    }

    public NavigationException(string message, Page page, Exception innerException) 
        : this(message, null, page, innerException)
    {
    }

    public NavigationException(string message, string navigationKey, Page page, Exception innerException) : base(message, innerException)
    {
        Page = page;
        NavigationKey = navigationKey;
    }

    public Page Page { get; }

    public string NavigationKey { get; }
}