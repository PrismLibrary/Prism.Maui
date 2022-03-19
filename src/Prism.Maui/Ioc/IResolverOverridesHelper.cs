namespace Prism.Ioc
{
    /// <summary>
    /// Provides a helper interface for Regions to be able to inject the current Region
    /// </summary>
    public interface IResolverOverridesHelper
    {
        IEnumerable<(Type Type, object Instance)> GetOverrides();
    }
}