using System;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace WinUISample.Pies.Processing;

public sealed partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
    }
}

public class PaintTaskToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is SolidColorPaint solidColor
            ? new SolidColorBrush(
                Color.FromArgb(
                    solidColor.Color.Alpha,
                    solidColor.Color.Red,
                    solidColor.Color.Green,
                    solidColor.Color.Blue))
            : null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
