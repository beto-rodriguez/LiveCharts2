using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Bars.WithBackground;

public partial class ViewModel : ObservableObject
{
    public ISeries[] Series { get; set; } =
    {
        new ColumnSeries<double>
        {
            IsHoverable = false, // disables the series from the tooltips // mark
            Values = new double[] { 10, 10, 10, 10, 10, 10, 10 },
            Stroke = null,
            Fill = new SolidColorPaint(new SKColor(30, 30, 30, 30)),
            IgnoresBarPosition = true
        },
        new ColumnSeries<double>
        {
            Values = new double[] { 3, 10, 5, 3, 7, 3, 8 },
            Stroke = null,
            Fill = new SolidColorPaint(SKColors.CornflowerBlue),
            IgnoresBarPosition = true
        }
    };

    public Axis[] YAxes { get; set; } =
    {
        new Axis { MinLimit = 0, MaxLimit = 10 }
    };
}
