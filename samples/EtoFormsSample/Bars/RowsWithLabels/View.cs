using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.Bars.RowsWithLabels;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var values1 = new int[] { 8, -3, 4 };
        var values2 = new int[] { 4, -6, 5 };
        var values3 = new int[] { 6, -9, 3 };

        var series = new ISeries[]
        {
            new RowSeries<int>
            {
                Values = values1,
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.End,
                DataLabelsSize = 14,
                ShowDataLabels = true
            },
            new RowSeries<int>
            {
                Values = values2,
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Middle,
                DataLabelsSize = 14,
                ShowDataLabels = true
            },
            new RowSeries<int>
            {
                Values = values3,
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Start,
                DataLabelsSize = 14,
                ShowDataLabels = true
            }
        };

        cartesianChart = new CartesianChart
        {
            Series = series,
        };

        Content = cartesianChart;
    }
}
