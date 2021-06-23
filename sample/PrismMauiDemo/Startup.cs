using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Prism;

namespace PrismMauiDemo
{
    public class Startup : IStartup
    {
        public void Configure(IAppHostBuilder appBuilder)
        {
            appBuilder
                .UsePrismApplication<App>(x => x.UseDryIoc())
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
        }
    }
}