using DryIoc;
using Microsoft.Maui;
using Prism.DryIoc;
using Prism.Ioc;

namespace Prism
{
    public sealed class DryIocPrismAppBuilder : PrismAppBuilder
    {
        public DryIocPrismAppBuilder(MauiAppBuilder builder) 
            : base(builder)
        {
        }

        /// <summary>
        /// Creates the <see cref="IContainerExtension"/> for DryIoc
        /// </summary>
        /// <returns></returns>
        protected override IContainerExtension CreateContainerExtension()
        {
            return new DryIocContainerExtension(new Container(DryIocContainerExtension.DefaultRules));
        }
    }
}
