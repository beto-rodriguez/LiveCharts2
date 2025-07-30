using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AvaloniaSample;

public class SampleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not string view
            ? null
            : (Activator.CreateInstance(null!, $"AvaloniaSample.{view.Replace('/', '.')}.View")?.Unwrap());
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
