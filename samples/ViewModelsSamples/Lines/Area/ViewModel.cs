using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Lines.Area;

public class ViewModel
{
    public List<ISeries> Series { get; set; } = new()
    {
        new LineSeries<double>
        {
            Values = new List<double> { -2, -1, 3, 5, 3, 4, 6 },
            // Set he Fill property to build an area series
            // by default the series has a fill color based on your app theme
            Fill = new SolidColorPaint(SKColors.CornflowerBlue),

            Stroke = null,
            GeometryFill = null,
            GeometryStroke = null
        }
    };

    // Creates a gray background and border in the draw margin.
    public DrawMarginFrame DrawMarginFrame => new()
    {
        Fill = new SolidColorPaint(new SKColor(220, 220, 220)),
        Stroke = new SolidColorPaint(new SKColor(180, 180, 180), 1)
    };
}
