using System;
using System.Globalization;
using System.Windows.Data;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace WPFSample.StepLines.Properties;

public class StringToPaintConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string s && !string.IsNullOrWhiteSpace(s))
        {
            // Try to parse as a color
            if (s.StartsWith("#"))
            {
                try
                {
                    var color = SKColor.Parse(s);
                    return new SolidColorPaint(color);
                }
                catch { }
            }
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
