using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using SkiaSharp;

namespace EtoFormsSample.Heat.Basic;

public class View : Panel
{
    public View()
    {
        var xLabels = new[] { "Charles", "Richard", "Ana", "Mari" };
        var yLabels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun" };
        var values = new WeightedPoint[]
        {
            // Charles
            new(0, 0, 150),
            new(0, 1, 123),
            new(0, 2, 310),
            new(0, 3, 225),
            new(0, 4, 473),
            new(0, 5, 373),
            // Richard
            new(1, 0, 432),
            new(1, 1, 312),
            new(1, 2, 135),
            new(1, 3, 78),
            new(1, 4, 124),
            new(1, 5, 423),
            // Ana
            new(2, 0, 543),
            new(2, 1, 134),
            new(2, 2, 524),
            new(2, 3, 315),
            new(2, 4, 145),
            new(2, 5, 80),
            // Mari
            new(3, 0, 90),
            new(3, 1, 123),
            new(3, 2, 70),
            new(3, 3, 123),
            new(3, 4, 432),
            new(3, 5, 142)
        };

        var series = new ISeries[]
        {
            new HeatSeries<WeightedPoint>
            {
                Values = values,
                HeatMap = [
                    SKColor.Parse("#FFF176").AsLvcColor(),
                    SKColor.Parse("#2F4F4F").AsLvcColor(),
                    SKColor.Parse("#0000FF").AsLvcColor()
                ]
            }
        };

        var xAxis = new Axis { Labels = xLabels };
        var yAxis = new Axis { Labels = yLabels };

        var cartesianChart = new CartesianChart
        {
            Series = series,
            XAxes = [xAxis],
            YAxes = [yAxis],
            LegendPosition = LiveChartsCore.Measure.LegendPosition.Right,
            Legend = new SKHeatLegend { BadgePadding = new LiveChartsCore.Drawing.Padding(30, 20, 8, 20) }
        };

        Content = cartesianChart;
    }
}
