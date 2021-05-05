using DryIoc;
using Prism.DryIoc;
using Prism.Ioc;

namespace Prism
{
    public static class DryIocContainerOptionsBuilderExtensions
    {
        public static void UseDryIoc(this ContainerOptionsBuilder options, Rules rules)
        {
            var container = new Container(rules);
            options.SetContainer(new DryIocContainerExtension(container));
        }

        public static void UseDryIoc(this ContainerOptionsBuilder options, IContainer container)
        {
            options.SetContainer(new DryIocContainerExtension(container));
        }

        public static void UseDryIoc(this ContainerOptionsBuilder options)
        {
            options.SetContainer(new DryIocContainerExtension());
        }
    }
}
