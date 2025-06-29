using System;
using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.Axes.DateTimeScaled;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var values = new DateTimePoint[]
        {
            new() { DateTime = new DateTime(2021, 1, 1), Value = 3 },
            new() { DateTime = new DateTime(2021, 1, 2), Value = 6 },
            new() { DateTime = new DateTime(2021, 1, 3), Value = 5 },
            new() { DateTime = new DateTime(2021, 1, 4), Value = 3 },
            new() { DateTime = new DateTime(2021, 1, 5), Value = 5 },
            new() { DateTime = new DateTime(2021, 1, 6), Value = 8 },
            new() { DateTime = new DateTime(2021, 1, 7), Value = 6 }
        };

        static string Formatter(DateTime date) => date.ToString("MMMM dd");

        var series = new ISeries[]
        {
            new ColumnSeries<DateTimePoint> { Values = values }
        };

        var xAxis = new DateTimeAxis(TimeSpan.FromDays(1), Formatter);

        cartesianChart = new CartesianChart
        {
            Series = series,
            XAxes = [xAxis],
        };

        Content = cartesianChart;
    }
}
