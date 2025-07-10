using System;
using Microsoft.UI.Xaml.Data;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace WinUISample.Axes.ColorsAndPosition;

public class StringToPaintConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string s && !string.IsNullOrWhiteSpace(s))
        {
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

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
