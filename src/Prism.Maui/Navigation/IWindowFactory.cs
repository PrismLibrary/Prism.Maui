using System;
namespace Prism.Navigation;

public interface IWindowFactory
{
    IEnumerable<Window> Windows { get; }

    void CreateWindow(Window window);
    void CloseWindow(Window window);
}

