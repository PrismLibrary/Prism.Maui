using Prism.Ioc;
using Prism.Navigation.Xaml;
using Prism.Regions;

namespace Prism.Behaviors;

internal class RegionCleanupBehavior : BehaviorBase<Page>
{
    private WeakReference<IRegion> _regionReference;

    public RegionCleanupBehavior(IRegion region)
    {
        _regionReference = new WeakReference<IRegion>(region);
    }

    public IRegion Region => _regionReference.TryGetTarget(out var target) ? target : null;

    protected override void OnDetachingFrom(Page bindable)
    {
        if (Region != null)
        {
            var container = bindable.GetContainerProvider();
            var manager = Region.RegionManager ?? container.Resolve<IRegionManager>();
            if (manager.Regions.ContainsRegionWithName(Region.Name))
            {
                manager.Regions.Remove(Region.Name);
            }
        }
    }
}
