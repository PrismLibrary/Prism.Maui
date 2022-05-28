using System.ComponentModel;
using Prism.Extensions;

namespace Prism.Behaviors;

internal class DelayedRegionCreationCallbackBehavior : Behavior<VisualElement>
{
    private Action _callback { get; }

    public DelayedRegionCreationCallbackBehavior(Action callback)
    {
        _callback = callback;
    }

    protected override void OnAttachedTo(VisualElement view)
    {
        if (view.TryGetParentPage(out var page))
        {
            _callback();
        }
        else
        {
            view.ParentChanged += OnParentChanged;
        }
    }

    private void OnParentChanged(object sender, EventArgs e)
    {
        var parent = (sender as VisualElement).Parent;
        Console.WriteLine($"Parent: {parent.GetType().Name}");
        if (sender is not VisualElement view || view.Parent is null)
            return;
        else if (view.TryGetParentPage(out var page))
            page.PropertyChanged += PagePropertyChanged;
        else
            view.Parent.ParentChanged += OnParentChanged;

        view.ParentChanged -= OnParentChanged;
    }

    private void PagePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (sender is not Page page)
            return;

        var container = page.GetValue(Navigation.Xaml.Navigation.NavigationScopeProperty);

        if(container is not null)
        {
            page.PropertyChanged -= PagePropertyChanged;
            _callback();
        }
    }
}
