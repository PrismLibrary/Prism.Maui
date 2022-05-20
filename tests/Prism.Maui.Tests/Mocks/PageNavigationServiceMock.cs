using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Prism.Behaviors;
using Prism.Common;
using Prism.Ioc;
using Prism.Navigation;

namespace Prism.Maui.Tests.Mocks
{
    public class PageNavigationServiceMock : PageNavigationService
    {
        IContainerExtension _containerMock;
        PageNavigationEventRecorder _recorder;

        public PageNavigationServiceMock(IContainerExtension containerMock, IApplication applicationProviderMock, PageNavigationEventRecorder recorder = null)
            : base(containerMock, applicationProviderMock)
        {
            _containerMock = containerMock;
            _recorder = recorder;
        }

        protected override Page CreatePage(string name)
        {
            var page = base.CreatePage(name);

            PageUtilities.InvokeViewAndViewModelAction<IPageNavigationEventRecordable>(
                page,
                x => x.PageNavigationEventRecorder = _recorder);

            return page;
        }
    }
}
