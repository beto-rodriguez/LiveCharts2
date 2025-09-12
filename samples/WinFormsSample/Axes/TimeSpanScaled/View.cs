using System;
using System.Windows.Forms;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinForms;

#pragma warning disable IDE1006 // Naming Styles

namespace WinFormsSample.Axes.TimeSpanScaled;

public partial class View : UserControl
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

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
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);
    }
}
