using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Prism.Common;
using Prism.Properties;

namespace Prism.Navigation
{
    public record ViewRegistration
    {
        public Type View { get; init; }
        public Type ViewModel { get; init; }
        public string Name { get; init; }
    }

    /// <summary>
    /// Provides a way for ViewModels involved in navigation to determine if a navigation request should continue.
    /// </summary>
    public interface IConfirmNavigation
    {
        /// <summary>
        /// Determines whether this instance accepts being navigated away from.
        /// </summary>
        /// <param name="parameters">The navigation parameters.</param>
        /// <returns><c>True</c> if navigation can continue, <c>False</c> if navigation is not allowed to continue</returns>
        bool CanNavigate(INavigationParameters parameters);
    }

    /// <summary>
    /// Provides a way for ViewModels involved in navigation to asynchronously determine if a navigation request should continue.
    /// </summary>
    public interface IConfirmNavigationAsync
    {
        /// <summary>
        /// Determines whether this instance accepts being navigated away from.
        /// </summary>
        /// <param name="parameters">The navigation parameters.</param>
        /// <returns><c>True</c> if navigation can continue, <c>False</c> if navigation is not allowed to continue</returns>
        Task<bool> CanNavigateAsync(INavigationParameters parameters);
    }

    /// <summary>
    /// Provides a way for the <see cref="INavigationService" /> to know whether the Flyout should be presented after navigation.
    /// </summary>
    public interface IFlyoutPageOptions
    {
        /// <summary>
        /// The INavigationService uses the result of this property to determine if the FlyoutPage.Flyout should be presented after navigation.
        /// </summary>
        bool IsPresentedAfterNavigation { get; }
    }

    public interface IInitialize
    {
        void Initialize(INavigationParameters parameters);
    }

    public interface IInitializeAsync
    {
        Task InitializeAsync(INavigationParameters parameters);
    }

    /// <summary>
    /// Provides a way for ViewModels involved in navigation to be notified of navigation activities after the target Page has been added to the navigation stack.
    /// </summary>
    public interface INavigatedAware
    {
        /// <summary>
        /// Called when the implementer has been navigated away from.
        /// </summary>
        /// <param name="parameters">The navigation parameters.</param>
        void OnNavigatedFrom(INavigationParameters parameters);

        /// <summary>
        /// Called when the implementer has been navigated to.
        /// </summary>
        /// <param name="parameters">The navigation parameters.</param>
        void OnNavigatedTo(INavigationParameters parameters);
    }

    /// <summary>
    /// Provides a way for ViewModels involved in navigation to be notified of navigation activities.
    /// </summary>
    public interface INavigationAware : INavigatedAware
    {

    }

    /// <summary>
    /// Provides a way for the INavigationService to make decisions regarding a NavigationPage during navigation.
    /// </summary>
    public interface INavigationPageOptions
    {
        /// <summary>
        /// The INavigationService uses the result of this property to determine if the NavigationPage should clear the NavigationStack when navigating to a new Page.
        /// </summary>
        /// <remarks>This is equivalent to calling PopToRoot, and then replacing the current Page with the target Page being navigated to.</remarks>
        bool ClearNavigationStackOnNavigation { get; }
    }

    /// <summary>
    /// Provides a way for the <see cref="INavigationService"/> to pass parameters during navigation.
    /// </summary>
    public interface INavigationParameters : IParameters
    {
    }

    /// <summary>
    /// Used to set internal parameters used by Prism's <see cref="INavigationService"/>
    /// </summary>
    public interface INavigationParametersInternal
    {
        /// <summary>
        /// Adds the key and value to the parameters Collection
        /// </summary>
        /// <param name="key">The key to reference this value in the parameters collection.</param>
        /// <param name="value">The value of the parameter to store</param>
        void Add(string key, object value);

        /// <summary>
        /// Checks collection for presence of key
        /// </summary>
        /// <param name="key">The key to check in the Collection</param>
        /// <returns><c>true</c> if key exists; else returns <c>false</c>.</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Returns the value of the member referenced by key
        /// </summary>
        /// <typeparam name="T">The type of object to be returned</typeparam>
        /// <param name="key">The key for the value to be returned</param>
        /// <returns>Returns a matching parameter of <typeparamref name="T"/> if one exists in the Collection</returns>
        T GetValue<T>(string key);
    }

    /// <summary>
    /// Common extensions for the <see cref="INavigationService"/>
    /// </summary>
    public static class INavigationServiceExtensions
    {
        /// <summary>
        /// Provides an easy to use way to provide an Error Callback without using await NavigationService
        /// </summary>
        /// <param name="navigationTask">The current Navigation Task</param>
        /// <param name="errorCallback">The <see cref="Exception"/> handler</param>
        public static void OnNavigationError(this Task<INavigationResult> navigationTask, Action<Exception> errorCallback)
        {
            navigationTask.Await(r =>
            {
                if (!r.Success)
                    errorCallback?.Invoke(r.Exception);
            });
        }

        /// <summary>
        /// When navigating inside a NavigationPage: Pops all but the root Page off the navigation stack
        /// </summary>
        /// <param name="navigationService">The INavigationService instance</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
        /// <remarks>Only works when called from a View within a NavigationPage</remarks>
        public static Task<INavigationResult> GoBackToRootAsync(this INavigationService navigationService, INavigationParameters parameters = null)
        {
            return navigationService.GoBackToRootAsync(parameters);
        }

        ///// <summary>
        ///// Gets an absolute path of the current page as it relates to it's position in the navigation stack.
        ///// </summary>
        ///// <returns>The absolute path of the current Page</returns>
        //public static string GetNavigationUriPath(this INavigationService navigationService)
        //{
        //    var currentpage = ((IPageAware)navigationService).Page;

        //    Stack<string> stack = new Stack<string>();
        //    currentpage = ProcessCurrentPageNavigationPath(currentpage, stack);
        //    ProcessNavigationPath(currentpage, stack);

        //    StringBuilder sb = new StringBuilder();
        //    while (stack.Count > 0)
        //    {
        //        sb.Append($"/{stack.Pop()}");
        //    }
        //    return sb.ToString();
        //}

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
        /// </summary>
        /// <param name="navigationService">Service for handling navigation between views</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
        public static Task<INavigationResult> GoBackAsync(this INavigationService navigationService, INavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            return navigationService.GoBackAsync(parameters, useModalNavigation, animated);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="name"/>.
        /// </summary>
        /// <param name="navigationService">Service for handling navigation between views</param>
        /// <param name="name">The name of the target to navigate to.</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PushModalAsync, if <c>false</c> uses PushAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
        public static Task<INavigationResult> NavigateAsync(this INavigationService navigationService, string name, INavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            return navigationService.NavigateAsync(name, parameters, useModalNavigation, animated);
        }

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
        /// </summary>
        /// <param name="navigationService">Service for handling navigation between views</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
        public static Task<INavigationResult> GoBackAsync(this INavigationService navigationService, params (string Key, object Value)[] parameters)
        {
            return navigationService.GoBackAsync(GetNavigationParameters(parameters));
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="name"/>.
        /// </summary>
        /// <param name="navigationService">Service for handling navigation between views</param>
        /// <param name="name">The Uri to navigate to</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
        /// <remarks>Navigation parameters can be provided in the Uri and by using the <paramref name="parameters"/>.</remarks>
        /// <example>
        /// NavigateAsync("MainPage?id=3&amp;name=dan", ("person", person), ("foo", bar));
        /// </example>
        public static Task<INavigationResult> NavigateAsync(this INavigationService navigationService, string name, params (string Key, object Value)[] parameters)
        {
            return navigationService.NavigateAsync(name, GetNavigationParameters(parameters));
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="uri"/>.
        /// </summary>
        /// <param name="navigationService">Service for handling navigation between views</param>
        /// <param name="uri">The Uri to navigate to</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
        /// <remarks>Navigation parameters can be provided in the Uri and by using the <paramref name="parameters"/>.</remarks>
        /// <example>
        /// NavigateAsync(new Uri("MainPage?id=3&amp;name=brian", UriKind.RelativeSource), parameters);
        /// </example>
        public static Task<INavigationResult> NavigateAsync(this INavigationService navigationService, Uri uri, INavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            return navigationService.NavigateAsync(uri, parameters, useModalNavigation, animated);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="uri"/>.
        /// </summary>
        /// <param name="navigationService">Service for handling navigation between views</param>
        /// <param name="uri">The Uri to navigate to</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
        /// <remarks>Navigation parameters can be provided in the Uri and by using the <paramref name="parameters"/>.</remarks>
        /// <example>
        /// NavigateAsync(new Uri("MainPage?id=3&amp;name=dan", UriKind.RelativeSource), ("person", person), ("foo", bar));
        /// </example>
        public static Task<INavigationResult> NavigateAsync(this INavigationService navigationService, Uri uri, params (string Key, object Value)[] parameters)
        {
            return navigationService.NavigateAsync(uri, GetNavigationParameters(parameters));
        }

        private static INavigationParameters GetNavigationParameters((string Key, object Value)[] parameters)
        {
            var navParams = new NavigationParameters();
            foreach (var (Key, Value) in parameters)
            {
                navParams.Add(Key, Value);
            }
            return navParams;
        }

        //private static void ProcessNavigationPath(Page page, Stack<string> stack)
        //{
        //    if (page.Parent is Page parent)
        //    {
        //        if (parent is NavigationPage)
        //        {
        //            var index = PageUtilities.GetCurrentPageIndex(page, page.Navigation.NavigationStack);
        //            if (index > 0)
        //            {
        //                int previousPageIndex = index - 1;
        //                for (int x = previousPageIndex; x >= 0; x--)
        //                {
        //                    AddSegmentToStack(page.Navigation.NavigationStack[x], stack);
        //                }
        //            }

        //            AddSegmentToStack(parent, stack);
        //        }
        //        else if (parent is FlyoutPage)
        //        {
        //            AddSegmentToStack(parent, stack);
        //        }

        //        ProcessNavigationPath(parent, stack);
        //    }
        //    else
        //    {
        //        ProcessModalNavigation(page, stack);
        //    }
        //}

        //private static void ProcessModalNavigation(Page page, Stack<string> stack)
        //{
        //    var index = PageUtilities.GetCurrentPageIndex(page, page.Navigation.ModalStack);
        //    int previousPageIndex = index - 1;
        //    for (int x = previousPageIndex; x >= 0; x--)
        //    {
        //        var childPage = page.Navigation.ModalStack[x];
        //        if (childPage is NavigationPage)
        //        {
        //            AddUseModalNavigationParameter(stack);
        //            ProcessModalNavigationPagePath((NavigationPage)childPage, stack);
        //        }
        //        else if (childPage is FlyoutPage flyout)
        //        {
        //            ProcessModalFlyoutPagePath(flyout, stack);
        //        }
        //        else
        //        {
        //            AddSegmentToStack(childPage, stack);
        //        }
        //    }

        //    ProcessMainPagePath(Application.Current?.MainPage, page, stack);
        //}

        //private static void ProcessMainPagePath(Page mainPage, Page previousPage, Stack<string> stack)
        //{
        //    if (mainPage == null)
        //        return;

        //    if (previousPage == mainPage)
        //        return;

        //    if (mainPage is NavigationPage)
        //    {
        //        AddUseModalNavigationParameter(stack);
        //        ProcessModalNavigationPagePath((NavigationPage)mainPage, stack);
        //    }
        //    else if (mainPage is FlyoutPage flyout)
        //    {
        //        var detail = flyout.Detail;
        //        if (detail is NavigationPage)
        //        {
        //            AddUseModalNavigationParameter(stack);
        //            ProcessModalNavigationPagePath((NavigationPage)detail, stack);
        //        }
        //        else
        //        {
        //            AddSegmentToStack(detail, stack);
        //        }

        //        AddSegmentToStack(mainPage, stack);
        //    }
        //    else
        //    {
        //        AddSegmentToStack(mainPage, stack);
        //    }
        //}

        //private static void ProcessModalNavigationPagePath(NavigationPage page, Stack<string> stack)
        //{
        //    var navStack = page.Navigation.NavigationStack.Reverse();
        //    foreach (var child in navStack)
        //    {
        //        AddSegmentToStack(child, stack);
        //    }

        //    AddSegmentToStack(page, stack);
        //}

        //private static void ProcessModalFlyoutPagePath(FlyoutPage page, Stack<string> stack)
        //{
        //    if (page.Detail is NavigationPage)
        //    {
        //        AddUseModalNavigationParameter(stack);
        //        ProcessModalNavigationPagePath((NavigationPage)page.Detail, stack);
        //    }
        //    else
        //    {
        //        AddSegmentToStack(page.Detail, stack);
        //    }

        //    AddSegmentToStack(page, stack);
        //}

        //private static Page ProcessCurrentPageNavigationPath(Page page, Stack<string> stack)
        //{
        //    var currentPageKeyInfo = PageNavigationRegistry.GetPageNavigationInfo(page.GetType());
        //    string currentSegment = $"{currentPageKeyInfo.Name}";

        //    if (page.Parent is Page parent)
        //    {
        //        var parentKeyInfo = PageNavigationRegistry.GetPageNavigationInfo(parent.GetType());

        //        if (parent is TabbedPage || parent is CarouselPage)
        //        {
        //            //set the selected tab to the current page
        //            currentSegment = $"{parentKeyInfo.Name}?{KnownNavigationParameters.SelectedTab}={currentPageKeyInfo.Name}";
        //            page = parent;
        //        }
        //        else if (parent is FlyoutPage)
        //        {
        //            currentSegment = $"{parentKeyInfo.Name}/{currentPageKeyInfo.Name}";
        //            page = parent;
        //        }
        //    }

        //    stack.Push(currentSegment);

        //    return page;
        //}

        //private static void AddSegmentToStack(Page page, Stack<string> stack)
        //{
        //    if (page == null)
        //        return;

        //    var keyInfo = PageNavigationRegistry.GetPageNavigationInfo(page.GetType());
        //    if (keyInfo != null)
        //        stack.Push(keyInfo.Name);
        //}

        private static void AddUseModalNavigationParameter(Stack<string> stack)
        {
            var lastPageName = stack.Pop();
            lastPageName = $"{lastPageName}?{KnownNavigationParameters.UseModalNavigation}=true";
            stack.Push(lastPageName);
        }
    }

    public static class NavigationParametersExtensions
    {
        public static NavigationMode GetNavigationMode(this INavigationParameters parameters)
        {
            var internalParams = (INavigationParametersInternal)parameters;
            if (internalParams.ContainsKey(KnownInternalParameters.NavigationMode))
                return internalParams.GetValue<NavigationMode>(KnownInternalParameters.NavigationMode);

            throw new System.ArgumentNullException(Resources.NavigationModeNotAvailable);
        }

        internal static INavigationParametersInternal GetNavigationParametersInternal(this INavigationParameters parameters)
        {
            return (INavigationParametersInternal)parameters;
        }
    }

    public enum PageNavigationSource
    {
        NavigationService,
        Device
    }
}
