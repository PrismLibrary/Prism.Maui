namespace Prism.Navigation;

internal class PrismWindow : Window
{
    public const string DefaultWindowName = "__PrismRootWindow";

    public PrismWindow(string name = DefaultWindowName)
    {
        Name = name;
    }

    public string Name { get; }
}
