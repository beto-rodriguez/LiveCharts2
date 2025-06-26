using Microsoft.UI.Xaml;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.WinUI;
using ViewModelsSamples.Pies.Icons;

namespace WinUISample.Pies.Icons;

public class CustomPieSeries : XamlPieSeries<double, DoughnutGeometry, SvgLabel>
{
    public CustomPieSeries()
    {
        PointMeasured +=
            point =>
            {
                var label = point.Label!;

                label.SvgString = Icon;
                label.Name = IconName;
                label.Size = 50;
                label.TranslateTransform = new(-25, -25);
            };
    }

    public static DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(string), typeof(CustomPieSeries), new PropertyMetadata(null));
    public static DependencyProperty IconNameProperty =
        DependencyProperty.Register(nameof(IconName), typeof(string), typeof(CustomPieSeries), new PropertyMetadata(null));

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string IconName
    {
        get => (string)GetValue(IconNameProperty);
        set => SetValue(IconNameProperty, value);
    }
}
