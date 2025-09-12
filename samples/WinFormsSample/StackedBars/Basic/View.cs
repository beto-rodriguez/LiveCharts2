using System;
using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.Kernel;

namespace WinFormsSample.StackedBars.Basic;

public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var values1 = new int[] { 3, 5, -3, 2, 5, -4, -2 };
        var values2 = new int[] { 4, 2, -3, 2, 3, 4, -2 };
        var values3 = new int[] { -2, 6, 6, 5, 4, 3, -2 };

        static string formatter(ChartPoint p) =>
            $"{p.Coordinate.PrimaryValue:N} ({p.StackedValue!.Share:P})";

        var series = new ISeries[]
        {
            new StackedColumnSeries<int>
            {
                Values = values1,
                ShowDataLabels = true,
                YToolTipLabelFormatter = formatter
            },
            new StackedColumnSeries<int>
            {
                Values = values2,
                ShowDataLabels = true,
                YToolTipLabelFormatter = formatter
            },
            new StackedColumnSeries<int>
            {
                Values = values3,
                ShowDataLabels = true,
                YToolTipLabelFormatter = formatter
            }
        };

        var cartesianChart = new CartesianChart
        {
            Series = series,
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);
    }
}
