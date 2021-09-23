using DryIoc;
using Microsoft.Maui.Controls.Hosting;
using Prism;
using Prism.DryIoc;

namespace Microsoft.Maui
{
    /// <summary>
    /// Application base class using DryIoc
    /// </summary>
    public static class PrismAppExtensions
    {
        public static PrismAppBuilder UsePrismApp<TApp>(this MauiAppBuilder builder)
            where TApp : PrismApplication
        {
            return builder.UsePrismApp<TApp>(new DryIocContainerExtension());
        }

        public static PrismAppBuilder UsePrismApp<TApp>(this MauiAppBuilder builder, Rules rules)
            where TApp : PrismApplication
        {
            return builder.UsePrismApp<TApp>(new DryIocContainerExtension(rules));
        }

        public static PrismAppBuilder UsePrismApp<TApp>(this MauiAppBuilder builder, global::DryIoc.IContainer container)
            where TApp : PrismApplication
        {
            return builder.UsePrismApp<TApp>(new DryIocContainerExtension(container));
        }
    }
}
