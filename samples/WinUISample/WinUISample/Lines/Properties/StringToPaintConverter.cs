using System;
using System.Globalization;
using Microsoft.UI.Xaml.Data;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace WinUISample.Lines.Properties;

public class StringToPaintConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string colorString)
        {
            return new SolidColorPaint(SKColor.Parse(colorString), 3);
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is SolidColorPaint paint)
        {
            return paint.Color.Red.ToString("X2") +
                   paint.Color.Green.ToString("X2") +
                   paint.Color.Blue.ToString("X2");
        }
        return null;
    }
}
