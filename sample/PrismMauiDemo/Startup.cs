using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Compatibility;
using Prism;
using Prism.DryIoc;

namespace PrismMauiDemo
{
    public class Startup : IStartup
    {
        public void Configure(IAppHostBuilder appBuilder)
        {
            appBuilder
                .UseFormsCompatibility()
                .UsePrismApplication<App>(x => { x.UseDryIoc(); });
        }
    }
}