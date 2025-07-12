using Microsoft.UI.Xaml.Data;
using Uno.Extensions.Navigation;

namespace UnoPlatformSample.Presentation;

public class IndexToContentConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not string route || string.IsNullOrEmpty(route))
            throw new ArgumentException("Value must be a non-empty string representing the route.");

        route = route.Replace('/', '.');
        var t = Type.GetType($"WinUISample.{route}.View") ?? throw new FileNotFoundException($"{route} not found!");
        return Activator.CreateInstance(t)!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
