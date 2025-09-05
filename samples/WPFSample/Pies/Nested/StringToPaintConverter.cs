using System;
using System.Globalization;
using System.Windows.Data;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace WPFSample.Pies.Nested;

public class StringToPaintConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string colorString)
            return new SolidColorPaint(SKColor.Parse(colorString));
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SolidColorPaint paint)
        {
            var c = paint.Color;
            return $"#{c.Red:X2}{c.Green:X2}{c.Blue:X2}";
        }
        return null;
    }
}
