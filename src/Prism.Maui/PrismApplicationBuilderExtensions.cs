using System;
using Prism.Ioc;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Hosting;

namespace Prism
{
    public static class PrismApplicationBuilderExtensions
    {
        public static IAppHostBuilder UsePrismApplication<T>(this IAppHostBuilder builder, Action<ContainerOptionsBuilder> configureContainer)
            where T : PrismApplication
        {
            var options = new ContainerOptionsBuilder();
            configureContainer(options);
            options.RegisterRequiredTypes();

            builder.UseMauiApp<T>()
                .UseServiceProviderFactory(new PrismServiceProviderFactory(options.Container));

            return builder;
        }
    }
}
