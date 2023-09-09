using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using LiveChartsCore.SkiaSharpView.Painting;

namespace AvaloniaSample.Pies.Processing;

public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

public class PaintTaskToBrushConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
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
