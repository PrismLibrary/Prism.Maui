# Prism.Maui

```cs
// OOB Maui Application
builder.UseMauiApp<App>();

// Prism.Maui
.UsePrismApplication<App>(x => x.UseDryIoc());
```

Note that the Prism `ContainerExtension` is created by the extension method on the ContainerOptionsBuilder. Overloads are available for you to pass in Rules or a `DryIoc.IContainer`. PrismApplication no longer resides in the DI project but in Platform project (Prism.Maui). The container is created before the PrismApplication instance.

## Service Registration

For existing Prism platforms, PrismApplication is responsible for a lot of service registration. Maui presents a very different paradigm to the app startup process as it uses the AppHostBuilder pattern that you may be familiar with from AspNetCore applications. Since a lot of the application wiring up occurs in the Startup before PrismApplication, we no longer register required services for Prism as part of the PrismApplication initialization.

Items which you only need within the context of what you are doing with Prism such as a ViewModel can be registered in either your Startup or PrismApplication. Views must be registered in PrismApplication.

## NOTE

Prism.Maui is current an experimental Alpha. Any preview build is largely meant to solicit additional developer feedback. APIs will likely change and break prior to being merged into the Prism repo and released as a fully official build.