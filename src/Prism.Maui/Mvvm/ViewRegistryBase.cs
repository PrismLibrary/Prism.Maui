using System.Text.RegularExpressions;
using Prism.Common;
using Prism.Ioc;
using Prism.Ioc.Internals;
using Prism.Navigation.Xaml;

namespace Prism.Mvvm;

public abstract class ViewRegistryBase : IViewRegistry
{
    private readonly IEnumerable<ViewRegistration> _registrations;
    private readonly IContainerInfo _containerInfo;
    private readonly ViewType _registryType;

    protected ViewRegistryBase(ViewType registryType, IContainerExtension container, IEnumerable<ViewRegistration> registrations)
    {
        _registrations = registrations;
        _containerInfo = container as IContainerInfo;
        _registryType = registryType;
    }

    public IEnumerable<ViewRegistration> Registrations => 
        _registrations.Where(x => x.Type == _registryType);

    public Type GetViewType(string name) =>
        GetRegistration(name)?.View;

    public object CreateView(IContainerProvider container, string name)
    {
        try
        {
            var registration = GetRegistration(name);
            if (registration is null)
                throw new KeyNotFoundException($"No view with the name '{name}' has been registered");

            var view = container.Resolve(registration.View) as BindableObject;

            view.SetContainerProvider(container);
            ConfigureView(view, container);

            if (registration.ViewModel is not null)
                view.SetValue(ViewModelLocator.ViewModelProperty, registration.ViewModel);

            Autowire(view);

            return view;
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Unable to create page '{name}'.", ex);
        }
    }

    private static IEnumerable<Type> GetCandidates(Type viewModelType)
    {
        var names = new[]
        {
            Regex.Replace(viewModelType.Name, @"ViewModel$", "Dialog"),
            Regex.Replace(viewModelType.Name, @"ViewModel$", "Page"),
            Regex.Replace(viewModelType.Name, @"ViewModel$", string.Empty),
            Regex.Replace(viewModelType.Name, @"Model$", string.Empty),
        }
        .Where(x => !x.EndsWith("PagePage"));

        var namespaces = new[]
        {
            viewModelType.Namespace.Replace("ViewModels", "Views"),
            viewModelType.Namespace.Replace("ViewModels", "Pages"),
            viewModelType.Namespace.Replace("ViewModels", "Dialogs")
        };

        var candidates = namespaces.Select(@namespace => names.Select(name => $"{@namespace}.{name}"))
            .SelectMany(x => x)
            .Select(x => viewModelType.AssemblyQualifiedName.Replace(viewModelType.FullName, x));
        return candidates
            .Select(x => Type.GetType(x, false))
            .Where(x => x is not null);
    }

    public string GetViewModelNavigationKey(Type viewModelType)
    {
        var registration = Registrations.LastOrDefault(x => x.ViewModel == viewModelType);
        if (registration is not null)
            return registration.Name;

        var candidates = GetCandidates(viewModelType);
        registration = Registrations.LastOrDefault(x => candidates.Any(c => c == x.View));
        if (registration is not null)
            return registration.Name;

        //    if (registrations.Count() > 1)
        //        throw new InvalidOperationException($"Multiple Registrations were found for '{viewModelType.FullName}'");
        //    else if (registrations.Count() == 1)
        //        return registrations.First().Name;

        //    throw new InvalidOperationException($"No Registrations were found for '{viewModelType.FullName}'");

        throw new KeyNotFoundException($"No View with the ViewModel '{viewModelType.Name}' has been registered");
    }

    public IEnumerable<ViewRegistration> ViewsOfType(Type baseType) =>
        Registrations.Where(x => x.View.IsAssignableFrom(typeof(TabbedPage)));

    public bool IsRegistered(string name) =>
        GetRegistration(name) is not null;

    protected ViewRegistration GetRegistration(string name) =>
        Registrations.LastOrDefault(x => x.Name == name);

    protected abstract void ConfigureView(BindableObject bindable, IContainerProvider container);

    protected void Autowire(BindableObject view)
    {
        if (view.BindingContext is not null)
            return;

        ViewModelLocator.Autowire(view);
    }

    //public static Type GetPageType(string name)
    //{
    //    var registrations = _registrations.Where(x => x.Name == name);
    //    if (!registrations.Any())
    //        throw new KeyNotFoundException(name);
    //    if (registrations.Count() > 1)
    //        throw new InvalidOperationException(string.Format(Resources.MultipleViewsRegisteredForNavigationKey, name, string.Join(", ", registrations.Select(x => x.View.FullName))));

    //    return registrations.First().View;
    //}
}
