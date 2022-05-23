using Prism.Behaviors;
using Prism.Common;
using Prism.Events;
using Prism.Ioc;
using Application = Microsoft.Maui.Controls.Application;

namespace Prism.Navigation;

/// <summary>
/// Provides page based navigation for ViewModels.
/// </summary>
public class PageNavigationService : INavigationService, IPageAware
{
    internal const string RemovePageRelativePath = "../";
    internal const string RemovePageInstruction = "__RemovePage/";
    internal const string RemovePageSegment = "__RemovePage";

    // TODO: Move this out of the PageNavigationService
    internal static PageNavigationSource NavigationSource { get; set; } = PageNavigationSource.Device;

    private readonly IContainerProvider _container;
    protected readonly IApplication _application;
    protected readonly IEventAggregator _eventAggregator;
    protected Window Window;

    protected Page _page;
    Page IPageAware.Page
    {
        get
        {
            if(Window is null)
            {
                Element curPage = _page;
                while (curPage?.Parent != null)
                {
                    if (curPage.Parent is Window window)
                        Window = window;
                    curPage = curPage.Parent;
                }
            }

            return _page;
        }
        set => _page = value;
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="PageNavigationService"/>.
    /// </summary>
    /// <param name="container">The <see cref="IContainerProvider"/> that will be used to resolve pages for navigation.</param>
    /// <param name="application">The <see cref="IApplication"/> that will let us ensure the Application.MainPage is set.</param>
    /// <param name="pageBehaviorFactory">The <see cref="IPageBehaviorFactory"/> that will apply base and custom behaviors to pages created in the <see cref="PageNavigationService"/>.</param>
    public PageNavigationService(IContainerProvider container, IApplication application, IEventAggregator eventAggregator)
    {
        _container = container;
        _application = application;
        _eventAggregator = eventAggregator;
    }

    /// <summary>
    /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
    /// </summary>
    /// <returns>If <c>true</c> a go back operation was successful. If <c>false</c> the go back operation failed.</returns>
    public virtual Task<INavigationResult> GoBackAsync()
    {
        return GoBackAsync(null);
    }

    /// <summary>
    /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
    /// </summary>
    /// <param name="parameters">The navigation parameters</param>
    /// <returns>If <c>true</c> a go back operation was successful. If <c>false</c> the go back operation failed.</returns>
    public virtual Task<INavigationResult> GoBackAsync(INavigationParameters parameters)
    {
        return GoBackInternal(parameters, null, true);
    }

    /// <summary>
    /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
    /// </summary>
    /// <param name="parameters">The navigation parameters</param>
    /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
    /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
    /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
    public virtual Task<INavigationResult> GoBackAsync(INavigationParameters parameters, bool? useModalNavigation, bool animated)
    {
        return GoBackInternal(parameters, useModalNavigation, animated);
    }

    /// <summary>
    /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
    /// </summary>
    /// <param name="parameters">The navigation parameters</param>
    /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
    /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
    /// <returns>If <c>true</c> a go back operation was successful. If <c>false</c> the go back operation failed.</returns>
    protected async virtual Task<INavigationResult> GoBackInternal(INavigationParameters parameters, bool? useModalNavigation, bool animated)
    {
        Page page = null;
        try
        {
            NavigationSource = PageNavigationSource.NavigationService;

            page = GetCurrentPage();
            if (IsRoot(GetPageFromWindow(), page))
                throw new NavigationException(NavigationException.CannotPopApplicationMainPage, page);

            var segmentParameters = UriParsingHelper.GetSegmentParameters(null, parameters);
            segmentParameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.Back);

            var canNavigate = await PageUtilities.CanNavigateAsync(page, segmentParameters);
            if (!canNavigate)
            {
                return new NavigationResult
                {
                    Exception = new NavigationException(NavigationException.IConfirmNavigationReturnedFalse, page)
                };
            }

            bool useModalForDoPop = UseModalGoBack(page, useModalNavigation);
            Page previousPage = PageUtilities.GetOnNavigatedToTarget(page, Window?.Page, useModalForDoPop);

            var poppedPage = await DoPop(page.Navigation, useModalForDoPop, animated);
            if (poppedPage != null)
            {
                PageUtilities.OnNavigatedFrom(page, segmentParameters);
                PageUtilities.OnNavigatedTo(previousPage, segmentParameters);
                PageUtilities.DestroyPage(poppedPage);

                return new NavigationResult { Success = true };
            }
        }
        catch (Exception ex)
        {
            return new NavigationResult { Exception = ex };
        }
        finally
        {
            NavigationSource = PageNavigationSource.Device;
        }

        return new NavigationResult
        {
            Exception = GetGoBackException(page, GetPageFromWindow())
        };
    }

    private static Exception GetGoBackException(Page currentPage, IView mainPage)
    {
        if (IsMainPage(currentPage, mainPage))
        {
            return new NavigationException(NavigationException.CannotPopApplicationMainPage, currentPage);
        }
        else if ((currentPage is NavigationPage navPage && IsOnNavigationPageRoot(navPage)) ||
            (currentPage.Parent is NavigationPage navParent && IsOnNavigationPageRoot(navParent)))
        {
            return new NavigationException(NavigationException.CannotGoBackFromRoot, currentPage);
        }

        return new NavigationException(NavigationException.UnknownException, currentPage);
    }

    private static bool IsOnNavigationPageRoot(NavigationPage navigationPage) =>
        navigationPage.CurrentPage == navigationPage.RootPage;

    private static bool IsMainPage(IView currentPage, IView mainPage)
    {
        if (currentPage == mainPage)
        {
            return true;
        }
        else if (mainPage is FlyoutPage flyout && flyout.Detail == currentPage)
        {
            return true;
        }
        else if (currentPage.Parent is TabbedPage tabbed && mainPage == tabbed)
        {
            return true;
        }
        else if (currentPage.Parent is NavigationPage navPage && navPage.CurrentPage == navPage.RootPage)
        {
            return IsMainPage(navPage, mainPage);
        }

        return false;
    }

    /// <summary>
    /// When navigating inside a NavigationPage: Pops all but the root Page off the navigation stack
    /// </summary>
    /// <param name="parameters">The navigation parameters</param>
    /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
    /// <remarks>Only works when called from a View within a NavigationPage</remarks>
    public virtual Task<INavigationResult> GoBackToRootAsync(INavigationParameters parameters)
    {
        return GoBackToRootInternal(parameters);
    }

    /// <summary>
    /// When navigating inside a NavigationPage: Pops all but the root Page off the navigation stack
    /// </summary>
    /// <param name="parameters">The navigation parameters</param>
    /// <remarks>Only works when called from a View within a NavigationPage</remarks>
    protected async virtual Task<INavigationResult> GoBackToRootInternal(INavigationParameters parameters)
    {
        try
        {
            if (parameters is null)
                parameters = new NavigationParameters();

            parameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.Back);

            var page = GetCurrentPage();
            var canNavigate = await PageUtilities.CanNavigateAsync(page, parameters);
            if (!canNavigate)
            {
                return new NavigationResult
                {
                    Exception = new NavigationException(NavigationException.IConfirmNavigationReturnedFalse, page)
                };
            }

            var pagesToDestroy = page.Navigation.NavigationStack.ToList(); // get all pages to destroy
            pagesToDestroy.Reverse(); // destroy them in reverse order
            var root = pagesToDestroy.Last();
            pagesToDestroy.Remove(root); //don't destroy the root page

            await page.Navigation.PopToRootAsync();

            foreach (var destroyPage in pagesToDestroy)
            {
                PageUtilities.OnNavigatedFrom(destroyPage, parameters);
                PageUtilities.DestroyPage(destroyPage);
            }

            PageUtilities.OnNavigatedTo(root, parameters);

            return new NavigationResult { Success = true };
        }
        catch (Exception ex)
        {
            return new NavigationResult { Exception = ex };
        }
    }

    /// <summary>
    /// Initiates navigation to the target specified by the <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the target to navigate to.</param>
    /// <param name="parameters">The navigation parameters</param>
    /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
    /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
    protected virtual Task<INavigationResult> NavigateInternal(string name, INavigationParameters parameters, bool? useModalNavigation, bool animated)
    {
        if (name.StartsWith(RemovePageRelativePath))
        {
            name = name.Replace(RemovePageRelativePath, RemovePageInstruction);
        }

        return NavigateInternal(UriParsingHelper.Parse(name), parameters, useModalNavigation, animated);
    }

    /// <summary>
    /// Initiates navigation to the target specified by the <paramref name="uri"/>.
    /// </summary>
    /// <param name="uri">The Uri to navigate to</param>
    /// <param name="parameters">The navigation parameters</param>
    /// <remarks>Navigation parameters can be provided in the Uri and by using the <paramref name="parameters"/>.</remarks>
    /// <example>
    /// NavigateAsync(new Uri("MainPage?id=3&amp;name=brian", UriKind.RelativeSource), parameters);
    /// </example>
    public virtual Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters parameters)
    {
        return NavigateInternal(uri, parameters, null, true);
    }

    /// <summary>
    /// Initiates navigation to the target specified by the <paramref name="uri"/>.
    /// </summary>
    /// <param name="uri">The Uri to navigate to</param>
    /// <param name="parameters">The navigation parameters</param>
    /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
    /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
    /// <remarks>Navigation parameters can be provided in the Uri and by using the <paramref name="parameters"/>.</remarks>
    /// <example>
    /// Navigate(new Uri("MainPage?id=3&amp;name=brian", UriKind.RelativeSource), parameters);
    /// </example>
    protected async virtual Task<INavigationResult> NavigateInternal(Uri uri, INavigationParameters parameters, bool? useModalNavigation, bool animated)
    {
        try
        {
            NavigationSource = PageNavigationSource.NavigationService;

            var navigationSegments = UriParsingHelper.GetUriSegments(uri);

            if (uri.IsAbsoluteUri)
            {
                await ProcessNavigationForAbsoluteUri(navigationSegments, parameters, useModalNavigation, animated);
                return new NavigationResult { Success = true };
            }
            else
            {
                await ProcessNavigation(GetCurrentPage(), navigationSegments, parameters, useModalNavigation, animated);
                return new NavigationResult { Success = true };
            }
        }
        catch (Exception ex)
        {
            return new NavigationResult { Exception = ex };
        }
        finally
        {
            NavigationSource = PageNavigationSource.Device;
        }
    }

    /// <summary>
    /// Processes the Navigation for the Queued navigation segments
    /// </summary>
    /// <param name="currentPage">The Current <see cref="Page"/> that we are navigating from.</param>
    /// <param name="segments">The Navigation <see cref="Uri"/> segments.</param>
    /// <param name="parameters">The <see cref="INavigationParameters"/>.</param>
    /// <param name="useModalNavigation"><see cref="Nullable{Boolean}"/> flag if we should force Modal Navigation.</param>
    /// <param name="animated">If <c>true</c>, the navigation will be animated.</param>
    /// <returns></returns>
    protected virtual async Task ProcessNavigation(Page currentPage, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
    {
        if (segments.Count == 0)
        {
            return;
        }

        var nextSegment = segments.Dequeue();

        var pageParameters = UriParsingHelper.GetSegmentParameters(nextSegment);
        if (pageParameters.ContainsKey(KnownNavigationParameters.UseModalNavigation))
        {
            useModalNavigation = pageParameters.GetValue<bool>(KnownNavigationParameters.UseModalNavigation);
        }

        if (nextSegment == RemovePageSegment)
        {
            await ProcessNavigationForRemovePageSegments(currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
            return;
        }

        if (currentPage is null)
        {
            await ProcessNavigationForRootPage(nextSegment, segments, parameters, useModalNavigation, animated);
            return;
        }

        if (currentPage is ContentPage)
        {
            await ProcessNavigationForContentPage(currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
        }
        else if (currentPage is NavigationPage nav)
        {
            await ProcessNavigationForNavigationPage(nav, nextSegment, segments, parameters, useModalNavigation, animated);
        }
        else if (currentPage is TabbedPage tabbed)
        {
            await ProcessNavigationForTabbedPage(tabbed, nextSegment, segments, parameters, useModalNavigation, animated);
        }
        else if (currentPage is FlyoutPage flyout)
        {
            await ProcessNavigationForFlyoutPage(flyout, nextSegment, segments, parameters, useModalNavigation, animated);
        }
    }

    protected virtual Task ProcessNavigationForRemovePageSegments(Page currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
    {
        if (!PageUtilities.HasDirectNavigationPageParent(currentPage))
        {
            throw new NavigationException(NavigationException.RelativeNavigationRequiresNavigationPage, currentPage);
        }

        return CanRemoveAndPush(segments)
            ? RemoveAndPush(currentPage, nextSegment, segments, parameters, useModalNavigation, animated)
            : RemoveAndGoBack(currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
    }

    private static bool CanRemoveAndPush(Queue<string> segments)
    {
        return !segments.All(x => x == RemovePageSegment);
    }

    private Task RemoveAndGoBack(Page currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
    {
        var pagesToRemove = new List<Page>();

        var currentPageIndex = currentPage.Navigation.NavigationStack.Count;
        if (currentPage.Navigation.NavigationStack.Count > 0)
        {
            currentPageIndex = currentPage.Navigation.NavigationStack.Count - 1;
        }

        while (segments.Count != 0)
        {
            currentPageIndex -= 1;
            pagesToRemove.Add(currentPage.Navigation.NavigationStack[currentPageIndex]);
            nextSegment = segments.Dequeue();
        }

        RemovePagesFromNavigationPage(currentPage, pagesToRemove);

        return GoBackAsync(parameters);
    }

    private async Task RemoveAndPush(Page currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
    {
        var pagesToRemove = new List<Page>
        {
            currentPage
        };

        var currentPageIndex = currentPage.Navigation.NavigationStack.Count;
        if (currentPage.Navigation.NavigationStack.Count > 0)
        {
            currentPageIndex = currentPage.Navigation.NavigationStack.Count - 1;
        }

        while (segments.Peek() == RemovePageSegment)
        {
            currentPageIndex -= 1;
            pagesToRemove.Add(currentPage.Navigation.NavigationStack[currentPageIndex]);
            nextSegment = segments.Dequeue();
        }

        await ProcessNavigation(currentPage, segments, parameters, useModalNavigation, animated);

        RemovePagesFromNavigationPage(currentPage, pagesToRemove);
    }

    private static void RemovePagesFromNavigationPage(Page currentPage, List<Page> pagesToRemove)
    {
        var navigationPage = (NavigationPage)currentPage.Parent;
        foreach (var page in pagesToRemove)
        {
            navigationPage.Navigation.RemovePage(page);
            PageUtilities.DestroyPage(page);
        }
    }

    protected virtual Task ProcessNavigationForAbsoluteUri(Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
    {
        return ProcessNavigation(null, segments, parameters, useModalNavigation, animated);
    }

    protected virtual async Task ProcessNavigationForRootPage(string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
    {
        var nextPage = CreatePageFromSegment(nextSegment);

        await ProcessNavigation(nextPage, segments, parameters, useModalNavigation, animated);

        var currentPage = GetPageFromWindow();
        var modalStack = currentPage?.Navigation.ModalStack.ToList();
        await DoNavigateAction(GetCurrentPage(), nextSegment, nextPage, parameters, async () =>
        {
            await DoPush(null, nextPage, useModalNavigation, animated);
        });
        if (currentPage != null)
        {
            PageUtilities.DestroyWithModalStack(currentPage, modalStack);
        }
    }

    protected virtual async Task ProcessNavigationForContentPage(Page currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
    {
        var nextPageType = NavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(nextSegment));
        bool useReverse = UseReverseNavigation(currentPage, nextPageType) && !(useModalNavigation.HasValue && useModalNavigation.Value);
        if (!useReverse)
        {
            var nextPage = CreatePageFromSegment(nextSegment);

            await ProcessNavigation(nextPage, segments, parameters, useModalNavigation, animated);

            await DoNavigateAction(currentPage, nextSegment, nextPage, parameters, async () =>
            {
                await DoPush(currentPage, nextPage, useModalNavigation, animated);
            });
        }
        else
        {
            await UseReverseNavigation(currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
        }
    }

    protected virtual async Task ProcessNavigationForNavigationPage(NavigationPage currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
    {
        if (currentPage.Navigation.NavigationStack.Count == 0)
        {
            await UseReverseNavigation(currentPage, nextSegment, segments, parameters, false, animated);
            return;
        }

        var clearNavigationStack = GetClearNavigationPageNavigationStack(currentPage);
        var isEmptyOfNavigationStack = currentPage.Navigation.NavigationStack.Count == 0;

        List<Page> destroyPages;
        if (clearNavigationStack && !isEmptyOfNavigationStack)
        {
            destroyPages = currentPage.Navigation.NavigationStack.ToList();
            destroyPages.Reverse();

            await currentPage.Navigation.PopToRootAsync(false);
        }
        else
        {
            destroyPages = new List<Page>();
        }

        var topPage = currentPage.Navigation.NavigationStack.LastOrDefault();
        var nextPageType = NavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(nextSegment));
        if (topPage?.GetType() == nextPageType)
        {
            if (clearNavigationStack)
                destroyPages.Remove(destroyPages.Last());

            if (segments.Count > 0)
                await UseReverseNavigation(topPage, segments.Dequeue(), segments, parameters, false, animated);

            await DoNavigateAction(topPage, nextSegment, topPage, parameters, onNavigationActionCompleted: (p) =>
            {
                if (nextSegment.Contains(KnownNavigationParameters.SelectedTab))
                {
                    var segmentParams = UriParsingHelper.GetSegmentParameters(nextSegment);
                    SelectPageTab(topPage, segmentParams);
                }
            });
        }
        else
        {
            await UseReverseNavigation(currentPage, nextSegment, segments, parameters, false, animated);

            if (clearNavigationStack && !isEmptyOfNavigationStack)
            {
                currentPage.Navigation.RemovePage(topPage);
            }
        }

        foreach (var destroyPage in destroyPages)
        {
            PageUtilities.DestroyPage(destroyPage);
        }
    }

    protected virtual async Task ProcessNavigationForTabbedPage(TabbedPage currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
    {
        var nextPage = CreatePageFromSegment(nextSegment);
        await ProcessNavigation(nextPage, segments, parameters, useModalNavigation, animated);
        await DoNavigateAction(currentPage, nextSegment, nextPage, parameters, async () =>
        {
            await DoPush(currentPage, nextPage, useModalNavigation, animated);
        });
    }

    protected virtual async Task ProcessNavigationForFlyoutPage(FlyoutPage currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
    {
        bool isPresented = GetFlyoutPageIsPresented(currentPage);

        var detail = currentPage.Detail;
        if (detail is null)
        {
            var newDetail = CreatePageFromSegment(nextSegment);
            await ProcessNavigation(newDetail, segments, parameters, useModalNavigation, animated);
            await DoNavigateAction(null, nextSegment, newDetail, parameters, onNavigationActionCompleted: (p) =>
            {
                currentPage.IsPresented = isPresented;
                currentPage.Detail = newDetail;
            });
            return;
        }

        if (useModalNavigation.HasValue && useModalNavigation.Value)
        {
            var nextPage = CreatePageFromSegment(nextSegment);
            await ProcessNavigation(nextPage, segments, parameters, useModalNavigation, animated);
            await DoNavigateAction(currentPage, nextSegment, nextPage, parameters, async () =>
            {
                currentPage.IsPresented = isPresented;
                await DoPush(currentPage, nextPage, true, animated);
            });
            return;
        }

        var nextSegmentType = NavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(nextSegment));

        //we must recreate the NavigationPage everytime or the transitions on iOS will not work properly, unless we meet the two scenarios below
        bool detailIsNavPage = false;
        bool reuseNavPage = false;
        if (detail is NavigationPage navPage)
        {
            detailIsNavPage = true;

            //we only care if we the next segment is also a NavigationPage.
            if (PageUtilities.IsSameOrSubclassOf<NavigationPage>(nextSegmentType))
            {
                //first we check to see if we are being forced to reuse the NavPage by checking the interface
                reuseNavPage = !GetClearNavigationPageNavigationStack(navPage);

                if (!reuseNavPage)
                {
                    //if we weren't forced to reuse the NavPage, then let's check the NavPage.CurrentPage against the next segment type as we don't want to recreate the entire nav stack
                    //just in case the user is trying to navigate to the same page which may be nested in a NavPage
                    var nextPageType = NavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(segments.Peek()));
                    var currentPageType = navPage.CurrentPage.GetType();
                    if (nextPageType == currentPageType)
                    {
                        reuseNavPage = true;
                    }
                }
            }
        }

        if ((detailIsNavPage && reuseNavPage) || (!detailIsNavPage && detail.GetType() == nextSegmentType))
        {
            await ProcessNavigation(detail, segments, parameters, useModalNavigation, animated);
            await DoNavigateAction(null, nextSegment, detail, parameters, onNavigationActionCompleted: (p) =>
            {
                if (detail is TabbedPage && nextSegment.Contains(KnownNavigationParameters.SelectedTab))
                {
                    var segmentParams = UriParsingHelper.GetSegmentParameters(nextSegment);
                    SelectPageTab(detail, segmentParams);
                }

                currentPage.IsPresented = isPresented;
            });
            return;
        }
        else
        {
            var newDetail = CreatePageFromSegment(nextSegment);
            await ProcessNavigation(newDetail, segments, parameters, newDetail is NavigationPage ? false : true, animated);
            await DoNavigateAction(detail, nextSegment, newDetail, parameters, onNavigationActionCompleted: (p) =>
            {
                if (detailIsNavPage)
                    OnNavigatedFrom(((NavigationPage)detail).CurrentPage, p);

                currentPage.IsPresented = isPresented;
                currentPage.Detail = newDetail;
                PageUtilities.DestroyPage(detail);
            });
            return;
        }
    }

    protected static bool GetFlyoutPageIsPresented(FlyoutPage page)
    {
        if (page is IFlyoutPageOptions flyoutPageOptions)
            return flyoutPageOptions.IsPresentedAfterNavigation;

        else if (page.BindingContext is IFlyoutPageOptions flyoutPageBindingContext)
            return flyoutPageBindingContext.IsPresentedAfterNavigation;

        return false;
    }

    protected static bool GetClearNavigationPageNavigationStack(NavigationPage page)
    {
        if (page is INavigationPageOptions iNavigationPage)
            return iNavigationPage.ClearNavigationStackOnNavigation;

        else if (page.BindingContext is INavigationPageOptions iNavigationPageBindingContext)
            return iNavigationPageBindingContext.ClearNavigationStackOnNavigation;

        return true;
    }

    protected static async Task DoNavigateAction(Page fromPage, string toSegment, Page toPage, INavigationParameters parameters, Func<Task> navigationAction = null, Action<INavigationParameters> onNavigationActionCompleted = null)
    {
        var segmentParameters = UriParsingHelper.GetSegmentParameters(toSegment, parameters);
        segmentParameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.New);

        var canNavigate = await PageUtilities.CanNavigateAsync(fromPage, segmentParameters);
        if (!canNavigate)
        {
            throw new NavigationException(NavigationException.IConfirmNavigationReturnedFalse, toPage);
        }

        await OnInitializedAsync(toPage, segmentParameters);

        if (navigationAction != null)
        {
            await navigationAction();
        }

        OnNavigatedFrom(fromPage, segmentParameters);

        onNavigationActionCompleted?.Invoke(segmentParameters);

        OnNavigatedTo(toPage, segmentParameters);
    }

    static async Task OnInitializedAsync(Page toPage, INavigationParameters parameters)
    {
        await PageUtilities.OnInitializedAsync(toPage, parameters);

        if (toPage is TabbedPage tabbedPage)
        {
            foreach (var child in tabbedPage.Children)
            {
                if (child is NavigationPage navigationPage)
                {
                    await PageUtilities.OnInitializedAsync(navigationPage.CurrentPage, parameters);
                }
                else
                {
                    await PageUtilities.OnInitializedAsync(child, parameters);
                }
            }
        }
    }

    private static void OnNavigatedTo(Page toPage, INavigationParameters parameters)
    {
        PageUtilities.OnNavigatedTo(toPage, parameters);

        if (toPage is TabbedPage tabbedPage)
        {
            if (tabbedPage.CurrentPage is NavigationPage navigationPage)
            {
                PageUtilities.OnNavigatedTo(navigationPage.CurrentPage, parameters);
            }
            else if (tabbedPage.BindingContext != tabbedPage.CurrentPage.BindingContext)
            {
                PageUtilities.OnNavigatedTo(tabbedPage.CurrentPage, parameters);
            }
        }
    }

    private static void OnNavigatedFrom(Page fromPage, INavigationParameters parameters)
    {
        PageUtilities.OnNavigatedFrom(fromPage, parameters);

        if (fromPage is TabbedPage tabbedPage)
        {
            if (tabbedPage.CurrentPage is NavigationPage navigationPage)
            {
                PageUtilities.OnNavigatedFrom(navigationPage.CurrentPage, parameters);
            }
            else if (tabbedPage.BindingContext != tabbedPage.CurrentPage.BindingContext)
            {
                PageUtilities.OnNavigatedFrom(tabbedPage.CurrentPage, parameters);
            }
        }
    }

    protected virtual Page CreatePage(string segmentName)
    {
        try
        {
            _container.CreateScope();
            var page = (Page)NavigationRegistry.CreateView(_container, segmentName);

            if (page is null)
                throw new NullReferenceException($"The resolved type for {segmentName} was null. You may be attempting to navigate to a Non-Page type");

            return page;
        }
        catch (Exception ex)
        {
            if (ex is NavigationException)
                throw;

            else if(ex is KeyNotFoundException)
                throw new NavigationException(NavigationException.NoPageIsRegistered, _page, ex);

            throw new NavigationException(NavigationException.ErrorCreatingPage, _page, ex);
        }
    }

    protected virtual Page CreatePageFromSegment(string segment)
    {
        string segmentName = UriParsingHelper.GetSegmentName(segment);
        var page = CreatePage(segmentName);
        if (page is null)
        {
            var innerException = new NullReferenceException(string.Format("{0} could not be created. Please make sure you have registered {0} for navigation.", segmentName));
            throw new NavigationException(NavigationException.NoPageIsRegistered, _page, innerException);
        }

        ConfigurePages(page, segment);

        return page;
    }

    void ConfigurePages(Page page, string segment)
    {
        if (page is TabbedPage)
        {
            ConfigureTabbedPage((TabbedPage)page, segment);
        }
    }

    void ConfigureTabbedPage(TabbedPage tabbedPage, string segment)
    {
        var parameters = UriParsingHelper.GetSegmentParameters(segment);

        var tabsToCreate = parameters.GetValues<string>(KnownNavigationParameters.CreateTab);
        if (tabsToCreate.Count() > 0)
        {
            foreach (var tabToCreate in tabsToCreate)
            {
                //created tab can be a single view or a view nested in a NavigationPage with the syntax "NavigationPage|ViewToCreate"
                var tabSegments = tabToCreate.Split('|');
                if (tabSegments.Length > 1)
                {
                    var navigationPage = CreatePageFromSegment(tabSegments[0]) as NavigationPage;
                    if (navigationPage != null)
                    {
                        var navigationPageChild = CreatePageFromSegment(tabSegments[1]);

                        navigationPage.PushAsync(navigationPageChild);

                        //when creating a NavigationPage w/ DI, a blank Page object is injected into the ctor. Let's remove it
                        if (navigationPage.Navigation.NavigationStack.Count > 1)
                            navigationPage.Navigation.RemovePage(navigationPage.Navigation.NavigationStack[0]);

                        //set the title because Xamarin doesn't do this for us.
                        navigationPage.Title = navigationPageChild.Title;
                        navigationPage.IconImageSource = navigationPageChild.IconImageSource;

                        tabbedPage.Children.Add(navigationPage);
                    }
                }
                else
                {
                    var tab = CreatePageFromSegment(tabToCreate);
                    tabbedPage.Children.Add(tab);
                }
            }
        }

        TabbedPageSelectTab(tabbedPage, parameters);
    }

    private static void SelectPageTab(Page page, INavigationParameters parameters)
    {
        if (page is TabbedPage tabbedPage)
        {
            TabbedPageSelectTab(tabbedPage, parameters);
        }
    }

    private static void TabbedPageSelectTab(TabbedPage tabbedPage, INavigationParameters parameters)
    {
        var selectedTab = parameters?.GetValue<string>(KnownNavigationParameters.SelectedTab);
        if (!string.IsNullOrWhiteSpace(selectedTab))
        {
            var selectedTabType = NavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(selectedTab));

            var childFound = false;
            foreach (var child in tabbedPage.Children)
            {
                if (!childFound && child.GetType() == selectedTabType)
                {
                    tabbedPage.CurrentPage = child;
                    childFound = true;
                }

                if (child is NavigationPage)
                {
                    if (!childFound && ((NavigationPage)child).CurrentPage.GetType() == selectedTabType)
                    {
                        tabbedPage.CurrentPage = child;
                        childFound = true;
                    }
                }
            }
        }
    }

    protected virtual async Task UseReverseNavigation(Page currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
    {
        var navigationStack = new Stack<string>();

        if (!string.IsNullOrWhiteSpace(nextSegment))
            navigationStack.Push(nextSegment);

        var illegalSegments = new Queue<string>();

        bool illegalPageFound = false;
        foreach (var item in segments)
        {
            //if we run into an illegal page, we need to create new navigation segments to properly handle the deep link
            if (illegalPageFound)
            {
                illegalSegments.Enqueue(item);
                continue;
            }

            //if any page decide to go modal, we need to consider it and all pages after it an illegal page
            var pageParameters = UriParsingHelper.GetSegmentParameters(item);
            if (pageParameters.ContainsKey(KnownNavigationParameters.UseModalNavigation))
            {
                if (pageParameters.GetValue<bool>(KnownNavigationParameters.UseModalNavigation))
                {
                    illegalSegments.Enqueue(item);
                    illegalPageFound = true;
                }
                else
                {
                    navigationStack.Push(item);
                }
            }
            else
            {
                var pageType = NavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(item));
                if (PageUtilities.IsSameOrSubclassOf<FlyoutPage>(pageType))
                {
                    illegalSegments.Enqueue(item);
                    illegalPageFound = true;
                }
                else
                {
                    navigationStack.Push(item);
                }
            }
        }

        var pageOffset = currentPage.Navigation.NavigationStack.Count;
        if (currentPage.Navigation.NavigationStack.Count > 2)
            pageOffset = currentPage.Navigation.NavigationStack.Count - 1;

        var onNavigatedFromTarget = currentPage;
        if (currentPage is NavigationPage navPage && navPage.CurrentPage != null)
            onNavigatedFromTarget = navPage.CurrentPage;

        bool insertBefore = false;
        while (navigationStack.Count > 0)
        {
            var segment = navigationStack.Pop();
            var nextPage = CreatePageFromSegment(segment);
            await DoNavigateAction(onNavigatedFromTarget, segment, nextPage, parameters, async () =>
            {
                await DoPush(currentPage, nextPage, useModalNavigation, animated, insertBefore, pageOffset);
            });
            insertBefore = true;
        }

        //if an illegal page is found, we force a Modal navigation
        if (illegalSegments.Count > 0)
            await ProcessNavigation(currentPage.Navigation.NavigationStack.Last(), illegalSegments, parameters, true, animated);
    }

    protected virtual Task DoPush(Page currentPage, Page page, bool? useModalNavigation, bool animated, bool insertBeforeLast = false, int navigationOffset = 0)
    {
        if (page is null)
            throw new ArgumentNullException(nameof(page));

        // Prevent Page from using Parent's ViewModel
        if (page.BindingContext is null)
            page.BindingContext = new object();

        if (currentPage is null)
        {
            var pageWindow = _page?.GetParentWindow();
            if (Window is null && pageWindow is not null)
                Window = pageWindow;
            else if (_application.Windows.OfType<PrismWindow>().Any(x => x.Name == PrismWindow.DefaultWindowName))
                Window = _application.Windows.OfType<PrismWindow>().First(x => x.Name == PrismWindow.DefaultWindowName);

            if (Window is null)
            {
                Window = new PrismWindow
                {
                    Page = page
                };
                ((List<Window>)_application.Windows).Add(Window as PrismWindow);
            }
            else
            {
                // BUG: https://github.com/dotnet/maui/issues/7275
                //Window.Page = page;

                // HACK: This is the only way CURRENTLY to ensure that the UI resets for Absolute Navigation
                var newWindow = new PrismWindow
                {
                    Page = page
                };
                _application.OpenWindow(newWindow);
                _application.CloseWindow(Window);
                Window = null;
            }

            return Task.FromResult<object>(null);
        }
        else
        {
            bool useModalForPush = UseModalNavigation(currentPage, useModalNavigation);

            if (useModalForPush)
            {
                return currentPage.Navigation.PushModalAsync(page, animated);
            }
            else
            {
                if (insertBeforeLast)
                {
                    return InsertPageBefore(currentPage, page, navigationOffset);
                }
                else
                {
                    return currentPage.Navigation.PushAsync(page, animated);
                }
            }
        }
    }

    protected virtual Task InsertPageBefore(Page currentPage, Page page, int pageOffset)
    {
        var firstPage = currentPage.Navigation.NavigationStack.Skip(pageOffset).FirstOrDefault();
        currentPage.Navigation.InsertPageBefore(page, firstPage);
        return Task.FromResult(true);
    }

    protected virtual Task<Page> DoPop(INavigation navigation, bool useModalNavigation, bool animated)
    {
        if (useModalNavigation)
            return navigation.PopModalAsync(animated);
        else
            return navigation.PopAsync(animated);
    }

    protected virtual Page GetCurrentPage()
    {
        return _page != null ? _page : GetPageFromWindow();
    }

    internal static bool UseModalNavigation(Page currentPage, bool? useModalNavigationDefault)
    {
        bool useModalNavigation = true;

        if (useModalNavigationDefault.HasValue)
            useModalNavigation = useModalNavigationDefault.Value;
        else if (currentPage is NavigationPage)
            useModalNavigation = false;
        else
            useModalNavigation = !PageUtilities.HasNavigationPageParent(currentPage);

        return useModalNavigation;
    }

    internal bool UseModalGoBack(Page currentPage, bool? useModalNavigationDefault)
    {
        if (useModalNavigationDefault.HasValue)
            return useModalNavigationDefault.Value;
        else if (currentPage is NavigationPage navPage)
            return GoBackModal(navPage);
        else if (PageUtilities.HasNavigationPageParent(currentPage, out var navParent))
            return GoBackModal(navParent);
        else
            return true;
    }

    private bool GoBackModal(NavigationPage navPage)
    {
        var rootPage = GetPageFromWindow();
        if (navPage.CurrentPage != navPage.RootPage)
            return false;
        else if (navPage.CurrentPage == navPage.RootPage && navPage.Parent is Application && rootPage != navPage)
            return true;
        else if (navPage.Parent is TabbedPage tabbed && tabbed != rootPage)
            return true;

        return false;
    }

    internal static bool UseReverseNavigation(Page currentPage, Type nextPageType)
    {
        return PageUtilities.HasNavigationPageParent(currentPage) && PageUtilities.IsSameOrSubclassOf<ContentPage>(nextPageType);
    }

    protected static bool IsRoot(Page mainPage, Page currentPage)
    {
        if (mainPage == currentPage)
            return true;

        return mainPage switch
        {
            FlyoutPage fp => IsRoot(fp.Detail, currentPage),
            TabbedPage tp => IsRoot(tp.CurrentPage, currentPage),
            NavigationPage np => IsRoot(np.RootPage, currentPage),
            _ => false
        };
    }

    private Page GetPageFromWindow()
    {
        try
        {
            return Window?.Page;
        }
#if DEBUG
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return null;
        }
#else
        catch
        {
            return null;
        }
#endif
    }
}