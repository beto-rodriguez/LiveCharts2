using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.Axes.CustomSeparatorsInterval;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var values = new int[] { 10, 55, 45, 68, 60, 70, 75, 78 };
        var customSeparators = new double[] { 0, 10, 25, 50, 100 };

        var series = new ISeries[]
        {
            new LineSeries<int> { Values = values }
        };

        var yAxis = new Axis
        {
            CustomSeparators = customSeparators
        };

        cartesianChart = new CartesianChart
        {
            Series = series,
            YAxes = [yAxis],
        };

        Content = cartesianChart;
    }
}
