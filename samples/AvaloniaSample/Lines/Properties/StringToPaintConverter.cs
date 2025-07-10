using System;
using System.Globalization;
using Avalonia.Data.Converters;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace AvaloniaSample.Lines.Properties;

public class StringToPaintConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string colorString)
            return new SolidColorPaint(SKColor.Parse(colorString));
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is SolidColorPaint paint)
        {
            var c = paint.Color;
            return $"#{c.Red:X2}{c.Green:X2}{c.Blue:X2}";
        }
        return null;
    }
}
