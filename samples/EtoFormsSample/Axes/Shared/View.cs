using System;
using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace EtoFormsSample.Axes.Shared;

public class View : Panel
{
    public View()
    {
        var values1 = Fetch(100);
        var values2 = Fetch(50);
        static string labeler(double value) => value.ToString("N2");
        var max = Math.Max(values1.Length, values2.Length);

        var sharedXAxis = new Axis
        {
            Labeler = labeler,
            MaxLimit = max,
            CrosshairPaint = new SolidColorPaint(SKColors.Red, 2),
            CrosshairLabelsBackground = SKColors.Red.AsLvcColor(),
            CrosshairLabelsPaint = new SolidColorPaint(SKColors.White)
        };
        var sharedXAxis2 = new Axis
        {
            Labeler = labeler,
            MaxLimit = max,
            CrosshairPaint = new SolidColorPaint(SKColors.Red, 2)
        };
        LiveChartsCore.SharedAxes.Set(sharedXAxis, sharedXAxis2);

        var cartesianChart = new CartesianChart
        {
            Series = new ISeries[]
            {
                new LineSeries<int> { Values = values1 }
            },
            ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X,
            XAxes = new[] { sharedXAxis }
        };

        var cartesianChart2 = new CartesianChart
        {
            Series = new ISeries[]
            {
                new ColumnSeries<int> { Values = values2 }
            },
            ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X,
            XAxes = new[] { sharedXAxis2 }
        };

        Content = new DynamicLayout(
            new DynamicRow(new DynamicControl { Control = cartesianChart, YScale = true }),
            new DynamicRow(new DynamicControl { Control = cartesianChart2, YScale = true })
        );
    }

    private static int[] Fetch(int length = 50)
    {
        var values = new int[length];
        var r = new Random();
        var t = 0;
        for (var i = 0; i < length; i++)
        {
            t += r.Next(-90, 100);
            values[i] = t;
        }
        return values;
    }
}
