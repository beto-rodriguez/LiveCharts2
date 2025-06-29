using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.Lines.XY;

public class View : Panel
{
    public View()
    {
        var values = new ObservablePoint[]
        {
            new(0, 4),
            new(1, 3),
            new(3, 8),
            new(18, 6),
            new(20, 12)
        };

        var series = new ISeries[]
        {
            new LineSeries<ObservablePoint>
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
