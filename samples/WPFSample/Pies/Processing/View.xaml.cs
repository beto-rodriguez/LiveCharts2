using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using LiveChartsCore.SkiaSharpView.Painting;

namespace WPFSample.Pies.Processing;

/// <summary>
/// Interaction logic for View.xaml
/// </summary>
public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
    }
}

public class PaintTaskToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
