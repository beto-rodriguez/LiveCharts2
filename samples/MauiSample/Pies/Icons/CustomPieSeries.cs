using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Maui;
using ViewModelsSamples.Pies.Icons;

namespace MauiSample.Pies.Icons;

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

    public static BindableProperty IconProperty = BindableProperty.Create(
        nameof(Icon), typeof(string), typeof(CustomPieSeries));

    public static BindableProperty IconNameProperty = BindableProperty.Create(
        nameof(IconName), typeof(string), typeof(CustomPieSeries));

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
