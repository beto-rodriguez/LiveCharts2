using System;
using System.Globalization;
using System.Windows.Data;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace WPFSample.Axes.ColorsAndPosition;

public class StringToPaintConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string colorString)
        {
            return new SolidColorPaint(SKColor.Parse(colorString));
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
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
