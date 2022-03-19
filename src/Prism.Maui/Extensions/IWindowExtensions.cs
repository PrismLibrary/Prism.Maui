using System.Reflection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Prism.Extensions
{
    public static class IWindowExtensions
    {
        public static bool SetPage(this IWindow window, Page page)
        {
            if(window is null)
                throw new ArgumentNullException(nameof(window));
            else if(page is null)
                throw new ArgumentNullException(nameof(page));

            try
            {
                if(window is Window mauiWindow)
                {
                    mauiWindow.Page = page;
                    return true;
                }

                var windowType = window.GetType();

                var setter = GetSetter(windowType.GetRuntimeProperties(), nameof(Page), nameof(IWindow.Content));

                if (setter != null)
                {
                    setter.Invoke(window, new[] { page });
                    return true;
                }

                var field = GetFirstField(windowType, nameof(Page), nameof(IWindow.Content));

                if (field != null)
                {
                    field.SetValue(window, page);
                    return true;
                }

                return false;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        private static MethodInfo GetSetter(IEnumerable<PropertyInfo> properties, params string[] propertyNames)
        {
            var prop = properties.FirstOrDefault(x => propertyNames.Any(p => p == x.Name) && x.SetMethod != null);

            return prop?.SetMethod;
        }

        private static FieldInfo GetFirstField(Type type, params string[] names)
        {
            foreach(var name in  names)
            {
                var field = GetField(type, name);
                if (field != null)
                    return field;
            }

            return null;
        }

        private static FieldInfo GetField(Type type, string name)
        {
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            var names = new[]
            {
                $"<{name}>k__BackingField",
                name.Camelcase(),
                $"_{name.Camelcase()}"
            };

            var field = fields.FirstOrDefault(x => names.Any(n => n == x.Name));

            if (field is null && type.BaseType is not null)
                return GetField(type.BaseType, name);

            return field;
        }

        private static string Camelcase(this string str) =>
            str.ToLowerInvariant()[0] + str.Substring(1);
    }
}
