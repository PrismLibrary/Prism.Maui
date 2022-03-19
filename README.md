# Prism.Maui

```cs
// OOB Maui Application
MauiApp.CreateBuilder()
    .UseMauiApp<App>();

// Prism.Maui
PrismApp.CreateBuilder()
    .RegisterServices(containerRegistry => {
        containerRegistry.RegisterForNavigation<MainPage>();
    })
    .ConfigureModuleCatalog(catalog => {
        catalog.AddModule<AuthModule>();
    })
    .UsePrismApp<App>();
```

## Service Registration

For existing Prism platforms, PrismApplication is responsible for a lot of service registration. Maui presents a very different paradigm to the app startup process as it uses the AppHostBuilder pattern that you may be familiar with from AspNetCore and other .NET Core applications. Due to the fact that services are registered as part of the MauiAppBuilder bootstrapping, the application bootstrapping that you would typically find as part of the PrismApplication is now instead part of PrismAppBuilder. This App Builder provides the typical PrismApplication bootstrapping, while returning the MauiAppBuilder when you call UsePrismApp to allow you to continue to configure your app like any other Maui Application.

For those items such as Pages registered for Navigation which need to be registered with the Prism Container Abstraction, you can register them with the RegisterServices extension off of the PrismAppBuilder. Similarly you can call ConfigureModuleCatalog to register modules or provide delegate to execute as part of the container initialization. Note that these will run prior to the Maui services being registered or the finial container being ready to use.

## NOTE

Prism.Maui is current an experimental Alpha. Any preview build is largely meant to solicit additional developer feedback. APIs will likely change and break prior to being merged into the Prism repo and released as a fully official build.