using DryIoc;
using Prism.Ioc;

namespace Prism.DryIoc
{
    public abstract class PrismApplication : PrismApplicationBase
    {
        protected override IContainerExtension CreateContainerExtension()
        {
            var container = new Container(GetDefaultRules());
            return new DryIocContainerExtension(container);
        }

        protected virtual Rules GetDefaultRules() => DryIocContainerExtension.DefaultRules;
    }
}
