using System;
using System.Globalization;
using Microsoft.UI.Xaml.Data;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace WinUISample.Pies.Nested;

public class StringToPaintConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string colorString)
            return new SolidColorPaint(SKColor.Parse(colorString));
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is SolidColorPaint paint)
        {
            var c = paint.Color;
            return $"#{c.Red:X2}{c.Green:X2}{c.Blue:X2}";
        }
        return null;
    }
}
