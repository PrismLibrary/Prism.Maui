using System;

namespace Prism.Navigation
{
    public record NavigationResult : INavigationResult
    {
        public bool Success { get; init; }

        public Exception Exception { get; init; }
    }
}
