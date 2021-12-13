using System;
using System.Globalization;
using LiveChartsCore.SkiaSharpView.Painting;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinSample.Pies.Processing;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class View : ContentPage
{
    public View()
    {
        InitializeComponent();
    }
}

public class PaintTaskToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is SolidColorPaint solidColor
            ? Color.FromRgba(
                    solidColor.Color.Red,
                    solidColor.Color.Green,
                    solidColor.Color.Blue,
                    solidColor.Color.Alpha)
            : null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
