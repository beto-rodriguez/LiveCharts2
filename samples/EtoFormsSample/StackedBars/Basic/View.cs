using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.StackedBars.Basic;

public class View : Panel
{
    public View()
    {
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
            Series = series
        };

        Content = cartesianChart;
    }
}
