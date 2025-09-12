using System;
using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace EtoFormsSample.Axes.ColorsAndPosition;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;
    private readonly ISeries[] series;
    private readonly Axis xAxis;
    private readonly Axis yAxis;
    private readonly Random random = new();

    public View()
    {
        // Sample data
        series =
        [
            new ColumnSeries<double> { Values = [2, 3, 8] }
        ];

        // Axis with initial position and color
        xAxis = new Axis
        {
            Position = AxisPosition.End,
            Name = "X Axis",
            LabelsPaint = new SolidColorPaint(SKColors.Red)
        };
        yAxis = new Axis
        {
            Name = "Y Axis"
        };

        cartesianChart = new CartesianChart
        {
            Series = series,
            XAxes = [xAxis],
            YAxes = [yAxis],
        };

        var b1 = new Button { Text = "toggle position" };
        b1.Click += (sender, e) =>
        {
            xAxis.Position = xAxis.Position == AxisPosition.End
                ? AxisPosition.Start
                : AxisPosition.End;
        };

        var b2 = new Button { Text = "new color" };
        b2.Click += (sender, e) =>
        {
            var r = (byte)random.Next(0, 255);
            var g = (byte)random.Next(0, 255);
            var b = (byte)random.Next(0, 255);
            xAxis.LabelsPaint = new SolidColorPaint(new SKColor(r, g, b));
        };

        var buttons = new StackLayout(b1, b2) { Orientation = Orientation.Horizontal, Padding = 2, Spacing = 4 };

        Content = new DynamicLayout(buttons, cartesianChart);
    }
}
