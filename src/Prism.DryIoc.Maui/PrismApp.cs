using DryIoc;

namespace Prism
{
    /// <summary>
    /// Application base class using DryIoc
    /// </summary>
    public sealed class PrismApp
    {
        public static PrismAppBuilder CreateBuilder()
        {
            return new DryIocPrismAppBuilder();
        }

        public static PrismAppBuilder CreateBuilder(IContainer container)
        {
            return new DryIocPrismAppBuilder(container);
        }

        public static PrismAppBuilder CreateBuilder(Rules rules)
{
            return new DryIocPrismAppBuilder(rules);
        }
    }
}
