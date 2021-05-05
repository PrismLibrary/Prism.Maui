# Prism.Maui

```cs
// OOB Maui Application
builder.UseMauiApp<App>();

// Prism.Maui
buidler.UsePrismApp<App>(options => options.UseDryIoc());
```

Note that the Prism `ContainerExtension` is created by the extension method on the ContainerOptionsBuilder. Overloads are available for you to pass in Rules or a `DryIoc.IContainer`. PrismApplication no longer resides in the DI project but in Platform project (Prism.Maui). The container is created before the PrismApplication instance.

## Service Registration

Currently PrismApplication is responsible for a lot of service registration. Since a lot of the application wiring up occurs in the Startup before PrismApplication additional things to consider would be to move the Required Prism services from PrismApplication to the ContainerOptionsBuilder. This needs to be done in a way that would mimick your ability to call or not call `base.RegisterRequiredTypes`...
