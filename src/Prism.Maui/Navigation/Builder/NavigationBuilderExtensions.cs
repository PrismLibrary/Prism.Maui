using System.ComponentModel;
using Prism.Navigation.Builder;

namespace Prism.Navigation;

public static class NavigationBuilderExtensions
{
    public static INavigationBuilder CreateBuilder(this INavigationService navigationService) =>
           new NavigationBuilder(navigationService);

    internal static string GetNavigationKey<TViewModel>()
    {
        var vmType = typeof(TViewModel);
        if (vmType.IsAssignableFrom(typeof(VisualElement)))
            throw new NavigationException(NavigationException.MvvmPatternBreak, typeof(TViewModel).Name);

        return NavigationRegistry.GetViewModelNavigationKey(vmType);
    }

    public static INavigationBuilder UseAbsoluteNavigation(this INavigationBuilder builder) =>
        builder.UseAbsoluteNavigation(true);

    public static INavigationBuilder AddNavigationSegment(this INavigationBuilder builder, string segmentName, bool? useModalNavigation = null) =>
        builder.AddNavigationSegment(segmentName, o =>
        {
            if (useModalNavigation.HasValue)
                o.UseModalNavigation(useModalNavigation.Value);
        });

    public static ICreateTabBuilder AddNavigationSegment<TViewModel>(this ICreateTabBuilder builder)
        where TViewModel : class, INotifyPropertyChanged =>
        builder.AddNavigationSegment<TViewModel>(b => { });

    public static ICreateTabBuilder AddNavigationSegment<TViewModel>(this ICreateTabBuilder builder, Action<ISegmentBuilder> configureSegment)
        where TViewModel : class, INotifyPropertyChanged =>
        builder.AddNavigationSegment(GetNavigationKey<TViewModel>(), configureSegment);

    public static INavigationBuilder AddNavigationSegment<TViewModel>(this INavigationBuilder builder)
        where TViewModel : class, INotifyPropertyChanged =>
        builder.AddNavigationSegment<TViewModel>(b => { });

    public static INavigationBuilder AddNavigationSegment<TViewModel>(this INavigationBuilder builder, Action<ISegmentBuilder> configureSegment)
        where TViewModel : class, INotifyPropertyChanged =>
        builder.AddNavigationSegment(GetNavigationKey<TViewModel>(), configureSegment);

    public static INavigationBuilder AddNavigationSegment<TViewModel>(this INavigationBuilder builder, bool useModalNavigation)
        where TViewModel : class, INotifyPropertyChanged =>
        builder.AddNavigationSegment<TViewModel>(b => b.UseModalNavigation(useModalNavigation));

    // Will check for the Navigation key of a registered NavigationPage
    public static INavigationBuilder AddNavigationPage(this INavigationBuilder builder) =>
        builder.AddNavigationPage(b => { });

    public static INavigationBuilder AddNavigationPage(this INavigationBuilder builder, Action<ISegmentBuilder> configureSegment)
    {
        var registrationInfo = NavigationRegistry.Registrations
            .FirstOrDefault(x => x.View.IsAssignableTo(typeof(NavigationPage)));
        if (registrationInfo is null)
            throw new NavigationException(NavigationException.NoPageIsRegistered, nameof(NavigationPage));

        return builder.AddNavigationSegment(registrationInfo.Name, configureSegment);
    }

    public static ICreateTabBuilder AddNavigationPage(this ICreateTabBuilder builder) =>
        builder.AddNavigationPage(b => { });

    public static ICreateTabBuilder AddNavigationPage(this ICreateTabBuilder builder, Action<ISegmentBuilder> configureSegment)
    {
        var registrationInfo = NavigationRegistry.Registrations
            .FirstOrDefault(x => x.View.IsAssignableTo(typeof(NavigationPage)));
        if (registrationInfo is null)
            throw new NavigationException(NavigationException.NoPageIsRegistered, nameof(NavigationPage));

        return builder.AddNavigationSegment(registrationInfo.Name, configureSegment);
    }

    public static INavigationBuilder AddNavigationPage(this INavigationBuilder builder, bool useModalNavigation) =>
        builder.AddNavigationPage(o => o.UseModalNavigation(useModalNavigation));

    //public static INavigationBuilder AddNavigationSegment(this INavigationBuilder builder, string segmentName, params string[] createTabs)
    //{
    //    return builder;
    //}

    //public static INavigationBuilder AddNavigationSegment(this INavigationBuilder builder, string segmentName, bool useModalNavigation, params string[] createTabs)
    //{
    //    return builder;
    //}

    //public static INavigationBuilder AddNavigationSegment(this INavigationBuilder builder, string segmentName, string selectTab, bool? useModalNavigation, params string[] createTabs)
    //{
    //    return builder;
    //}

    public static async void Navigate(this INavigationBuilder builder)
    {
        await builder.NavigateAsync();
    }

    public static async void Navigate(this INavigationBuilder builder, Action<Exception> onError)
    {
        await builder.NavigateAsync(onError);
    }

    public static async void Navigate(this INavigationBuilder builder, Action onSuccess)
    {
        await builder.NavigateAsync(onSuccess, _ => { });
    }

    public static async void Navigate(this INavigationBuilder builder, Action onSuccess, Action<Exception> onError)
    {
        await builder.NavigateAsync(onSuccess, onError);
    }

    public static ISegmentBuilder UseModalNavigation(this ISegmentBuilder builder) =>
        builder.UseModalNavigation(true);

    public static ICreateTabBuilder AddNavigationSegment(this ICreateTabBuilder builder, string segmentNameOrUri) =>
            builder.AddNavigationSegment(segmentNameOrUri, null);

    public static ITabbedSegmentBuilder CreateTab(this ITabbedSegmentBuilder builder, string segmentName, Action<ISegmentBuilder> configureSegment) =>
        builder.CreateTab(o => o.AddNavigationSegment(segmentName, configureSegment));

    public static ITabbedSegmentBuilder CreateTab(this ITabbedSegmentBuilder builder, string segmentNameOrUri) =>
        builder.CreateTab(o => o.AddNavigationSegment(segmentNameOrUri));

    public static ITabbedSegmentBuilder CreateTab<TViewModel>(this ITabbedSegmentBuilder builder)
        where TViewModel : class, INotifyPropertyChanged
    {
        var navigationKey = GetNavigationKey<TViewModel>();
        return builder.CreateTab(navigationKey);
    }

    public static ITabbedSegmentBuilder SelectTab<TViewModel>(this ITabbedSegmentBuilder builder)
        where TViewModel : class, INotifyPropertyChanged
    {
        var navigationKey = GetNavigationKey<TViewModel>();
        return builder.SelectedTab(navigationKey);
    }

    public static ITabbedNavigationBuilder SelectTab(this ITabbedNavigationBuilder builder, params string[] navigationSegments) =>
        builder.SelectTab(string.Join("|", navigationSegments));
}
