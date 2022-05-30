using Prism.Common;

namespace Prism.DryIoc.Maui.Tests.Mocks;

public class MockViewBViewModel : MockViewModelBase
{
    public MockViewBViewModel(IPageAccessor pageAccessor, INavigationService navigationService) : base(pageAccessor, navigationService)
    {
    }
}
