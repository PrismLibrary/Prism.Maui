using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace PrismMauiDemo
{
    public class MainWindow : VisualElement, IWindow
    {
        public MainWindow()
        {
            View = new MainPage();
        }

        public IView View { get; set; }
    }
}