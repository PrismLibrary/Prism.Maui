using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Prism
{
    public class PrismApplicationWindow : VisualElement, IWindow
    {
        public IView Content { get; set; }
        public string Title { get; set; }
    }
}
