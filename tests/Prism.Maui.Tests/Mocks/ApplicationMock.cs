using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Prism.Maui.Tests.Mocks
{
    public class ApplicationMock : IApplication
    {
        private List<IWindow> _windows = new List<IWindow> { new PrismApplicationWindow() };

        public ApplicationMock(Page page = null)
        {
            if (page != null)
                _windows[0].View = page;
        }

        public VisualElement MainPage => _windows[0].View as VisualElement;

        public IReadOnlyList<IWindow> Windows => _windows;

        public IWindow CreateWindow(IActivationState activationState)
        {
            throw new NotImplementedException();
        }
    }
}
