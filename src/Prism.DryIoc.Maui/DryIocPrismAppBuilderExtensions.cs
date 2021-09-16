using DryIoc;
using Prism.DryIoc;
using IContainer = DryIoc.IContainer;

namespace Prism
{
    public static class DryIocPrismAppBuilderExtensions
    {
        public static PrismAppBuilder WithContainerExtension(this PrismAppBuilder builder, IContainer container) =>
            builder.WithContainerExtension(new DryIocContainerExtension(container));

        public static PrismAppBuilder WithContainerExtension(this PrismAppBuilder builder, Rules rules) =>
            builder.WithContainerExtension(new Container(rules));
    }
}
