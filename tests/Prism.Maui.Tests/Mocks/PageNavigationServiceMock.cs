using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Prism.Behaviors;
using Prism.Common;
using Prism.Events;
using Prism.Ioc;
using Prism.Navigation;

namespace Prism.Maui.Tests.Mocks
{
    public class PageNavigationServiceMock : PageNavigationService
    {
        IContainerProvider _containerMock;
        PageNavigationEventRecorder _recorder;

        public PageNavigationServiceMock(IContainerProvider container, 
            IApplication application, 
            IEventAggregator eventAggregator, 
            IPageAccessor pageAccessor,
            PageNavigationEventRecorder recorder = null) 
            : base(container, application, eventAggregator, pageAccessor)
        {
            _containerMock = container;
            _recorder = recorder;
        }

        protected override Page CreatePage(string name)
        {
            var page = base.CreatePage(name);

            MvvmHelpers.InvokeViewAndViewModelAction<IPageNavigationEventRecordable>(
                page,
                x => x.PageNavigationEventRecorder = _recorder);

            return page;
        }
    }
}
