using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Lines.Area;

public class ViewModel
{
    public ISeries[] Series { get; set; } = [
        new LineSeries<double>
        {
            Values = [-2, -1, 3, 5, 3, 4, 6],
            // Set he Fill property to build an area series
            // by default the series has a fill color based on your app theme
            Fill = new SolidColorPaint(SKColors.CornflowerBlue), // mark

            Stroke = null,
            GeometryFill = null,
            GeometryStroke = null
        }
    ];

    // Creates a gray background and border in the draw margin.
    public DrawMarginFrame DrawMarginFrame => new()
    {
        Fill = new SolidColorPaint(new SKColor(220, 220, 220)),
        Stroke = new SolidColorPaint(new SKColor(180, 180, 180), 1)
    };
}
