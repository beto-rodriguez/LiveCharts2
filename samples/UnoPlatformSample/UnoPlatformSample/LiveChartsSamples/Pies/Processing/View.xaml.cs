using System;
using LiveChartsCore.SkiaSharpView.Painting;
using Windows.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace UnoWinUISample.Pies.Processing;

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
        var solidColor = (SolidColorPaint)value;
        return new SolidColorBrush(
            Color.FromArgb(
                solidColor.Color.Alpha,
                solidColor.Color.Red,
                solidColor.Color.Green,
                solidColor.Color.Blue));
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
