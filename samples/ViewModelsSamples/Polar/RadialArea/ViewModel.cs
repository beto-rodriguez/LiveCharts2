using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ViewModelsSamples.Polar.RadialArea;

public partial class ViewModel : ObservableObject
{
    public ISeries[] Series { get; set; } =
    {
        new PolarLineSeries<int>
        {
            Values = new[] { 7, 5, 7, 5, 6 },
            LineSmoothness = 0,
            GeometrySize= 0,
            Fill = new SolidColorPaint(SKColors.Blue.WithAlpha(90))
        },
        new PolarLineSeries<int>
        {
            Values = new[] { 2, 7, 5, 9, 7 },
            LineSmoothness = 1,
            GeometrySize = 0,
            Fill = new SolidColorPaint(SKColors.Red.WithAlpha(90))
        }
    };

    public PolarAxis[] AngleAxes { get; set; } =
    {
        new PolarAxis
        {
            LabelsRotation = LiveCharts.TangentAngle,
            Labels = new[] { "first", "second", "third", "forth", "fifth" }
        }
    };
}
