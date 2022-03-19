using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Prism.Extensions;
using Xunit;

namespace Prism.Maui.Tests.Fixtures
{
    public class WindowExtensionFixture
    {
        [Fact]
        public void SetsWindowContent()
        {
            var page = new ContentPage();
            var window = new Window();
            Assert.True(window.SetPage(page));
        }
    }
}
