using System.ComponentModel;

namespace Prism.Ioc
{
    public class ContainerOptionsBuilder
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IContainerExtension Container { get; private set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetContainer(IContainerExtension container)
        {
            ContainerLocator.SetContainerExtension(() => container);
            Container = ContainerLocator.Current;
        }
    }
}
