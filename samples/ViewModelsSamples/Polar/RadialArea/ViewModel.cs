using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Polar.RadialArea;

public class ViewModel
{
    public ISeries[] Series { get; set; } = [
        new PolarLineSeries<int>
        {
            Values = [7, 5, 7, 5, 6],
            LineSmoothness = 0,
            GeometrySize= 0,
            Fill = new SolidColorPaint(SKColors.Blue.WithAlpha(90))
        },
        new PolarLineSeries<int>
        {
            Values = [2, 7, 5, 9, 7],
            LineSmoothness = 1,
            GeometrySize = 0,
            Fill = new SolidColorPaint(SKColors.Red.WithAlpha(90))
        }
    ];

    public PolarAxis[] AngleAxes { get; set; } = [
        new PolarAxis
        {
            LabelsRotation = LiveCharts.TangentAngle,
            Labels = ["first", "second", "third", "forth", "fifth"]
        }
    ];
}
