using System;
using Prism.Ioc;
using Microsoft.Maui.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Maui.Controls;
using Prism.Mvvm;

namespace Prism
{
    public static class PrismApplicationBuilderExtensions
    {
        public static IAppHostBuilder UsePrismApplication<T>(this IAppHostBuilder builder, Action<ContainerOptionsBuilder> configureContainer)
            where T : PrismApplication
        {
            var options = new ContainerOptionsBuilder();
            configureContainer(options);

            builder.UseMauiApp<T>()
                .UseServiceProviderFactory(new PrismServiceProviderFactory(options.Container));

            return builder;
        }
    }

    
}
