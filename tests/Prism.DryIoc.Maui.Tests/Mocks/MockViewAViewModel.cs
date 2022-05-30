using Prism.Common;

namespace Prism.DryIoc.Maui.Tests.Mocks;

public class MockViewAViewModel : MockViewModelBase
{
    public MockViewAViewModel(IPageAccessor pageAccessor, INavigationService navigationService) : base(pageAccessor, navigationService)
    {
    }
}
