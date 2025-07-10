using System;
using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.Axes.TimeSpanScaled;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var values = new TimeSpanPoint[]
        {
            new() { TimeSpan = TimeSpan.FromMilliseconds(1), Value = 10 },
            new() { TimeSpan = TimeSpan.FromMilliseconds(2), Value = 6 },
            new() { TimeSpan = TimeSpan.FromMilliseconds(3), Value = 3 },
            new() { TimeSpan = TimeSpan.FromMilliseconds(4), Value = 12 },
            new() { TimeSpan = TimeSpan.FromMilliseconds(5), Value = 8 }
        };

        static string Formatter(TimeSpan value) => $"{value:fff}ms";

        var series = new ISeries[]
        {
            new ColumnSeries<TimeSpanPoint> { Values = values }
        };

        var xAxis = new TimeSpanAxis(TimeSpan.FromMilliseconds(1), Formatter);

        cartesianChart = new CartesianChart
        {
            Series = series,
            XAxes = [xAxis],
        };

        Content = cartesianChart;
    }
}
