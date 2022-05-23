using Prism.Behaviors;

namespace Prism.Ioc;

public static class BehaviorFactoryRegistrationExtensions
{
    /// <summary>
    /// Registers a provided action for
    /// </summary>
    /// <param name="container"></param>
    /// <param name="pageBehaviorFactory"></param>
    /// <returns></returns>
    public static IContainerRegistry RegisterPageBehaviorFactory(this IContainerRegistry container, Action<Page> pageBehaviorFactory)
    {
        return container.RegisterInstance<IPageBehaviorFactory>(new DelegatePageBehaviorFactory(pageBehaviorFactory));
    }

    public static IContainerRegistry RegisterPageBehaviorFactory(this IContainerRegistry container, Action<IContainerProvider, Page> pageBehaviorFactory) =>
        container.RegisterScoped<IPageBehaviorFactory>(c => new DelegateContainerPageBehaviorFactory(pageBehaviorFactory, c));

    public static IContainerRegistry RegisterPageBehavior<TBehavior>(this IContainerRegistry container)
        where TBehavior : Behavior =>
        container
            .Register<TBehavior>()
            .RegisterPageBehaviorFactory((c, p) => p.Behaviors.Add(c.Resolve<TBehavior>()));

    /// <summary>
    /// This will apply the <typeparamref name="TBehavior"/> to the <see cref="Page"/>, when it is a <typeparamref name="TPage"/>.
    /// </summary>
    /// <typeparam name="TPage">The type of Page</typeparam>
    /// <typeparam name="TBehavior">The type of Behavior</typeparam>
    /// <param name="container"></param>
    /// <returns></returns>
    public static IContainerRegistry RegisterPageBehavior<TPage, TBehavior>(this IContainerRegistry container)
        where TPage : Page
        where TBehavior : Behavior =>
        container
            .Register<TBehavior>()
            .RegisterPageBehaviorFactory((c, p) =>
            {
                if (p is TPage)
                    p.Behaviors.Add(c.Resolve<TBehavior>());
            });
}

