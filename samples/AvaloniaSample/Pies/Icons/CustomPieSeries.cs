using Avalonia;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace AvaloniaSample.Pies.Icons;

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

    public static AvaloniaProperty IconProperty =
        AvaloniaProperty.Register<CustomPieSeries, string>(nameof(Icon));
    public static AvaloniaProperty IconNameProperty =
        AvaloniaProperty.Register<CustomPieSeries, string>(nameof(IconName));

    public string Icon
    {
        get => (string)GetValue(IconProperty)!;
        set => SetValue(IconProperty, value);
    }

    public string IconName
    {
        get => (string)GetValue(IconNameProperty)!;
        set => SetValue(IconNameProperty, value);
    }
}
