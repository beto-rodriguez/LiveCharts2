using System.Collections.Generic;
using SkiaSharp;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;

namespace ViewModelsSamples.Axes.MatchScale;

// in this example we are going to learn how to match the // mark
// scale of both axes, this means that both X and Y axes will take // mark
// the same amount of space in the screen per unit of data // mark

public class ViewModel
{
    public ISeries[] Series { get; set; } = [
        new LineSeries<ObservablePoint>
        {
            Values = Fetch(),
            Stroke = new SolidColorPaint(new SKColor(33, 150, 243), 4),
            Fill = null,
            GeometrySize = 0
        }
    ];

    // we are forcing the step to be 0.1 // mark
    // but this is just to highlight that both axes use the same scale // mark

    public ICartesianAxis[] XAxes { get; set; } = [
        new Axis
        {
            Name = "X axis",
            SeparatorsPaint = new SolidColorPaint(new SKColor(220, 220, 200)),
            MinStep = 0.1,
            ForceStepToMin = true
        }
    ];

    public ICartesianAxis[] YAxes { get; set; } = [
        new Axis
        {
            Name = "Y axis",
            SeparatorsPaint = new SolidColorPaint(new SKColor(200, 200, 200)),
            MinStep = 0.1,
            ForceStepToMin = true
        }
    ];

    public DrawMarginFrame Frame { get; set; } =
        new() { Stroke = new SolidColorPaint(new SKColor(200, 200, 200), 2) };

    private static List<ObservablePoint> Fetch()
    {
        var list = new List<ObservablePoint>();
        var fx = EasingFunctions.BounceInOut;

        for (var x = 0f; x < 1f; x += 0.001f)
        {
            var y = fx(x);

            list.Add(new()
            {
                X = x - 0.5,
                Y = y - 0.5
            });
        }

        return list;
    }
}
