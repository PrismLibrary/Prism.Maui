using DryIoc;
using Prism.DryIoc;
using Prism.Ioc;

namespace Prism
{
    public sealed class DryIocPrismAppBuilder : PrismAppBuilder
    {
        public DryIocPrismAppBuilder()
        {
        }

        public DryIocPrismAppBuilder(IContainer container)
            : base(new DryIocContainerExtension(container))
        {
        }

        public DryIocPrismAppBuilder(Rules rules)
            : this(new Container(rules))
        {
        }

        /// <summary>
        /// Creates the <see cref="IContainerExtension"/> for DryIoc
        /// </summary>
        /// <returns></returns>
        protected override IContainerExtension CreateContainerExtension()
        {
            return new DryIocContainerExtension();
        }
    }
}
