using SkiaSharp;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Kernel.Sketches;

namespace ViewModelsSamples.Axes.Crosshairs;

public class ViewModel
{
    public ISeries[] Series { get; set; } = [
        new LineSeries<double> { Values = [200, 558, 458, 249, 457, 339, 587] },
        new LineSeries<double> { Values = [210, 400, 300, 350, 219, 323, 618] },
    ];

    public ICartesianAxis[] XAxes { get; set; } = [
        new Axis
        {
            CrosshairLabelsBackground = SKColors.DarkOrange.AsLvcColor(),
            CrosshairLabelsPaint = new SolidColorPaint(SKColors.DarkRed),
            CrosshairPaint = new SolidColorPaint(SKColors.DarkOrange, 1),
            Labeler = value => value.ToString("N2")
        }
    ];

    public ICartesianAxis[] YAxes { get; set; } = [
        new Axis
        {
            CrosshairLabelsBackground = SKColors.DarkOrange.AsLvcColor(),
            CrosshairLabelsPaint = new SolidColorPaint(SKColors.DarkRed),
            CrosshairPaint = new SolidColorPaint(SKColors.DarkOrange, 1),
            CrosshairSnapEnabled = true // snapping is also supported
        }
    ];
}
