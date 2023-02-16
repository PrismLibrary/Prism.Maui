# Prism.Maui

**NOTE** This repository is now in an archive state. All further work on Prism.Maui can be found in the main [Prism repo](https://github.com/PrismLibrary/Prism). Please open any issues there.

Prism for .NET MAUI is more than simply a port of Prism for Xamarin.Forms. Many of the features remain largely untouched, however the codebase has been written specifically for MAUI. For existing Prism platforms, PrismApplication is responsible for a lot of service registration. Maui presents a very different paradigm to the app startup process as it uses the AppHostBuilder pattern that you may be familiar with from AspNetCore and other .NET Core applications. Due to the fact that services are registered as part of the MauiAppBuilder bootstrapping, the application bootstrapping that you would typically find as part of the PrismApplication is now instead part of PrismAppBuilder.

As part of this rewrite, some attention to making it easier for legacy Xamarin.Forms apps to be ported to MAUI has been given. In general however, this has been written with specific attention to the updated application initialization patterns that we see with .NET MAUI.

## Support

Please help support Prism and Prism.Maui by becoming a GitHub Sponsor. The work on Prism.Maui is NOT funded by any entity and is done entirely in my spare time. Your financial contributions help support my efforts to provide the best possible experience for the community.

## Using Prism.Maui

To help follow MAUI's App Builder pattern we have introduced the `UsePrism<TApp>` extension on the `MauiAppBuilder`. This will create a `PrismAppBuilder` which gives you specific access to various methods you may want for initializing your application.

```cs
// OOB Maui Application
MauiApp.CreateBuilder()
    .UseMauiApp<App>();

// Prism.Maui
MauiApp.CreateBuilder()
    .UseMauiApp<App>()
    .UsePrism(prism => {
        // Register Services and setup initial Navigation
    });
```

Some of the methods available on the `PrismAppBuilder` are going to seem a bit familiar for developers coming from Prism for Xamarin.Forms such as:

```cs
MauiApp.CreateBuilder()
    .UseMauiApp<App>()
    .UsePrism(prism =>
        prism.RegisterServices(container => {
            container.Register<ISomeService, SomeImplementation>();
            container.RegisterForNavigation<ViewA, ViewAViewModel>();
        })
        .ConfigureModuleCatalog(catalog => {
            catalog.AddModule<ModuleA>();
        });
        .OnInitialized(() => {
            // Do some initializations here
        })
    );
```

You will find that this includes useful extensions that consider that you are wiring up these initializations as part of the App Builder. A common case would be where you may have used the container in PrismApplication in the past, you now have an overload that provides the use of the Container.

```cs
MauiApp.CreateBuilder()
    .UseMauiApp<App>()
    .UsePrism(prism =>
        prism.OnInitialized(container => {
            var foo = container.Resolve<IFoo>();
            // Do some initializations here
        })
    );
```

The `PrismAppBuilder` additionally provides some new things to make your life easier.

```cs
MauiApp.CreateBuilder()
    .UseMauiApp<App>()
    .UsePrism(prism =>
        prism.OnAppStart(async navigationService =>
        {
            var result = await navigationService.NavigateAsync("MainPage/NavigationPage/ViewA");
            if (!result.Success)
            {
                System.Diagnostics.Debugger.Break();
            }
        })
    );
```

### Microsoft Extensions Support

To help make it even easier we have added some special extensions to the `PrismAppBuilder` that will make it easier to use Microsoft Extensions. This includes the `ConfigureLogging` method to make it easier to setup your logging while still using the `PrismAppBuilder`. For those who prefer to use `IServiceCollection` to manage their registrations, you can additionally use the `ConfigureServices` extension and bypass the normal Prism `RegisterServices` method.

```cs
MauiApp.CreateBuilder()
    .UseMauiApp<App>()
    .UsePrism(prism =>
        prism.ConfigureServices(services => {
            services.AddSingleton<IFoo, Foo>();
            services.RegisterForNavigation<ViewA, ViewAViewModel>();
        })
    );
```

## Upgrading from Prism.Forms

PrismApplication is largely obsolete for Prism.Maui. The PrismAppBuilder does not have an explicit requirement on it. To make it easier on those who are upgrading, we do have legacy support methods to make updating your existing apps a little easier. This includes a `RegisterTypes` & `OnInitialized` method which will get called during the app initialization. It is recommended however that you migrate this code to your App Builder.

## Navigation Builders

Creating a complex navigation stack via a URI can be intimidating for some. For others it can pose a challenge as they require a dynamically built navigation stack. And still other developers have often requested a ViewModel first approach to navigation in Prism. The NavigationBuilder is a great way to help solve these various problems. The NavigationBuilder can be created using an extension on the INavigationService. There is no way to clear the NavigationBuilder, so you should NOT attempt to store this as a property on your ViewModel, and you should instead create a new instance whenever you need to navigate. The samples below are meant to show you some of what is possible. It is not in any way meant as guidance for creating a navigation stack using the most completely random approach possible. Pick a standard approach and follow it. The Builders are meant to expose an API that is easy to use and can help people with different preferences for how to build their navigation stack.

```cs
// Shown with option OnError callback
navigationService.CreateBuilder()
    .AddNavigationSegment("MainPage")
    .AddNavigationPage()
    .AddNavigationSegment<ViewAViewModel>()
    .AddNavigationSegment("ViewB")
    .Navigate(HandleNavigationError);

// This returns the INavigationResult... 
// Also available with overloads to provide OnSuccess & OnError callbacks
navigationService.CreateBuilder()
    .AddTabbedSegment(b =>
    {
        b.CreateTab(t => t.AddNavigationSegment<ViewAViewModel>())
            .CreateTab("ViewB")
            .CreateTab(t => t.AddNavigationPage().AddNavigationSegment<ViewCViewModel>())
            .SelectTab<ViewBViewModel>();
    })
    .NavigateAsync();
```

It's also important to note that the builders will make some attempt to stop you from breaking the MVVM pattern and you will get an exception if you attempt to do:

```cs
navigationService.CreateBuilder()
    .AddNavigationSegment<ViewA>()
    .Navigate();
```


