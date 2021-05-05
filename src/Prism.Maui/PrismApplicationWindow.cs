using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Prism
{
    public class PrismApplicationWindow : Window
    {
        public PrismApplicationWindow()
            : base(null)
        {
        }

        public PrismApplicationWindow(Page page)
            : base(page)
        {
        }

        public IMauiContext MauiContext { get; set; }
    }
}
