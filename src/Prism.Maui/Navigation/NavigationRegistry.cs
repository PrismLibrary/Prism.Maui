using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Microsoft.Maui.Controls;
using Prism.Ioc;
using Prism.Mvvm;

namespace Prism.Navigation
{
    public static class NavigationRegistry
    {
        private static readonly List<ViewRegistration> _registrations = new List<ViewRegistration>();

        public static void Register<TView, TViewModel>(string name) =>
            Register(typeof(TView), typeof(TViewModel), name);

        public static void Register(Type viewType, Type viewModelType, string name)
        {
            if (_registrations.Any(x => x.Name == name))
                throw new DuplicateNameException($"A view with the name '{name}' has already been registered");

            var registration = new ViewRegistration
            {
                View = viewType,
                ViewModel = viewModelType,
                Name = name
            };
            _registrations.Add(registration);
        }

        public static object CreateView(IContainerProvider container, string name)
        {
            var registration = _registrations.FirstOrDefault(x => x.Name == name);
            if (registration is null)
                throw new KeyNotFoundException($"No view with the name '{name}' has been registered");

            var view = container.Resolve(registration.View);
            if (view is BindableObject bindable && bindable.BindingContext is null && (bool)bindable.GetValue(ViewModelLocator.AutowireViewModelProperty))
            {
                // TODO: Use ViewModelLocationProvider if the ViewModel type is null...
                var viewModelType = registration.ViewModel;
                bindable.BindingContext = container.Resolve(viewModelType);
            }

            return view;
        }

        public static Type GetPageType(string name) =>
            _registrations.FirstOrDefault(x => x.Name == name)?.View;

        public static ViewRegistration GetPageNavigationInfo(Type viewType) => 
            _registrations.FirstOrDefault(x => x.View == viewType);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ClearRegistrationCache() => _registrations.Clear();
    }
}
