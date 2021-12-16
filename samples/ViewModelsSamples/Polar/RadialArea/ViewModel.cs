using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Polar.RadialArea;

public class ViewModel
{
    public ISeries[] Series { get; set; }
        = {
            new PolarLineSeries<int>
            {
                Values = new[] { 7, 5, 2, 4, 7, 7, 5, 3, 4 },
                LineSmoothness = 1,
                GeometrySize = 0,
                Fill = new SolidColorPaint(SKColors.Blue.WithAlpha(90))
            },
            new PolarLineSeries<int>
            {
                Values = new[] { 4, 6, 3, 7, 5, 6, 3, 5, 6 },
                LineSmoothness = 1,
                GeometrySize= 0,
                Fill = new SolidColorPaint(SKColors.Red.WithAlpha(90))
            }
        };

    public PolarAxis[] AngleAxes { get; set; }
        = {
            new PolarAxis
            {
                //Labels = new[] { "Category 1", "Category 2", "Category 3", "Category 4", "Category 5" }
            }
        };
}
