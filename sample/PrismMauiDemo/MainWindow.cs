using Microsoft.Maui;

namespace PrismMauiDemo
{
    public class MainWindow : IWindow
    {
        public MainWindow()
        {
            Page = new MainPage();
        }

        public IPage Page { get; set; }

        public IMauiContext MauiContext { get; set; }
    }
}