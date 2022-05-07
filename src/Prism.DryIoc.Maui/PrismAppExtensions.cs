using DryIoc;
using Prism;
using Prism.DryIoc;

namespace Microsoft.Maui;

/// <summary>
/// Application base class using DryIoc
/// </summary>
public static class PrismAppExtensions
{
    public static PrismAppBuilder UsePrismApp<TApp>(this MauiAppBuilder builder)
        where TApp : Application
    {
        return builder.UsePrismApp<TApp>(new DryIocContainerExtension());
    }

    public static PrismAppBuilder UsePrismApp<TApp>(this MauiAppBuilder builder, Rules rules)
        where TApp : Application
    {
        rules = rules.WithTrackingDisposableTransients()
            .With(Made.Of(FactoryMethod.ConstructorWithResolvableArguments))
            .WithFactorySelector(Rules.SelectLastRegisteredFactory());
        return builder.UsePrismApp<TApp>(new DryIocContainerExtension(rules));
    }
}
