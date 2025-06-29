using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.Scatter.Basic;

public class View : Panel
{
    public View()
    {
        var values = new ObservablePoint[]
        {
            new(2.2, 5.4),
            new(3.6, 9.6),
            new(9.9, 5.2),
            new(8.1, 4.7),
            new(5.3, 7.1)
        };

        var series = new ISeries[]
        {
            new ScatterSeries<ObservablePoint>
            {
                Values = values
            }
        };

        var cartesianChart = new CartesianChart
        {
            Series = series
        };

        Content = cartesianChart;
    }
}
