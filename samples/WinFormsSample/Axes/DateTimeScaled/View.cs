using System;
using System.Windows.Forms;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinForms;

#pragma warning disable IDE1006 // Naming Styles

namespace WinFormsSample.Axes.DateTimeScaled;

public partial class View : UserControl
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

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
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);
    }
}
