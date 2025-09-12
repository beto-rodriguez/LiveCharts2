using System.Globalization;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace MauiSample.Pies.Nested;

public class StringToPaintConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        new SolidColorPaint(SKColor.Parse((string?)value));

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is SolidColorPaint paint
            ? paint.Color.Red.ToString("X2") +
              paint.Color.Green.ToString("X2") +
              paint.Color.Blue.ToString("X2")
            : (object?)null;
}
