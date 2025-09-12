using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.Bars.Layered;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var values1 = new int[] { 6, 3, 5, 7, 3, 4, 6, 3 };
        var values2 = new int[] { 2, 4, 8, 9, 5, 2, 4, 7 };

        var series = new ISeries[]
        {
            new ColumnSeries<int>
            {
                Values = values1,
                MaxBarWidth = 999999,
                IgnoresBarPosition = true
            },
            new ColumnSeries<int>
            {
                Values = values2,
                MaxBarWidth = 30,
                IgnoresBarPosition = true
            }
        };

        cartesianChart = new CartesianChart
        {
            Series = series,
        };

        Content = cartesianChart;
    }
}
