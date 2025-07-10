using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.Lines.Straight;

public class View : Panel
{
    public View()
    {
        var values1 = new double[] { 5, 0, 5, 0, 5, 0 };
        var values2 = new double[] { 7, 2, 7, 2, 7, 2 };

        var series = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = values1,
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 0
            },
            new LineSeries<double>
            {
                Values = values2,
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 1
            }
        };

        var cartesianChart = new CartesianChart
        {
            Series = series
        };

        Content = cartesianChart;
    }
}
