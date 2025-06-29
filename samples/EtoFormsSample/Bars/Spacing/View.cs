using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.Bars.Spacing;

public class View : Panel
{
    public View()
    {
        var values = new double[] { 20, 50, 40, 20, 40, 30, 50, 20, 50, 40 };

        var series = new ISeries[]
        {
            new ColumnSeries<double>
            {
                Values = values,
                Padding = 0,
                MaxBarWidth = 99999
            }
        };

        var cartesianChart = new CartesianChart
        {
            Series = series,
        };

        Content = cartesianChart;
    }
}
