using System;
using Microsoft.Extensions.DependencyInjection;
using Prism.Navigation;

namespace Prism;

public class PrismApplication : Application, IWindowFactory
{
    IEnumerable<Window> IWindowFactory.Windows => Windows;
    private Window _initialWindow;

    protected sealed override Window CreateWindow(IActivationState activationState)
    {
        if (_initialWindow is not null)
            return _initialWindow;
        else if (Windows.OfType<PrismWindow>().Any())
            return _initialWindow = Windows.OfType<PrismWindow>().First();

        activationState.Context.Services.GetRequiredService<PrismAppBuilder>().OnAppStarted();

        return _initialWindow;
    }

    void IWindowFactory.CreateWindow(Window window)
    {
        if (_initialWindow is null)
            _initialWindow = window;
        else
            OpenWindow(window);
    }
}

