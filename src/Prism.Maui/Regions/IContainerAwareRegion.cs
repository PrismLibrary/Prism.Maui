using System.ComponentModel;
using Prism.Ioc;

namespace Prism.Regions;

[EditorBrowsable(EditorBrowsableState.Never)]
public interface IContainerAwareRegion : IRegion
{
    IContainerProvider Container { get; }
}
