using Microsoft.Maui;

namespace Prism
{
    /// <summary>
    /// Application base class using DryIoc
    /// </summary>
    public sealed class PrismApp
    {
        public static PrismAppBuilder CreateBuilder()
        {
            var builder = MauiApp.CreateBuilder();
            return new DryIocPrismAppBuilder(builder);
        }
    }
}
