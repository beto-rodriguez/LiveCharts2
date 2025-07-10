using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.StackedBars.Groups;

public class View : Panel
{
    public View()
    {
        var values1 = new int[] { 3, 5, 3 };
        var values2 = new int[] { 4, 2, 3 };
        var values3 = new int[] { 4, 6, 6 };
        var values4 = new int[] { 2, 5, 4 };
        var labels = new string[] { "Category 1", "Category 2", "Category 3" };

        var series = new ISeries[]
        {
            new StackedColumnSeries<int> { Values = values1, StackGroup = 0 },
            new StackedColumnSeries<int> { Values = values2, StackGroup = 0 },
            new StackedColumnSeries<int> { Values = values3, StackGroup = 1 },
            new StackedColumnSeries<int> { Values = values4, StackGroup = 1 }
        };

        var xAxes = new Axis[]
        {
            new() {
                LabelsRotation = -15,
                Labels = labels,
                MinStep = 1,
                ForceStepToMin = true
            }
        };

        var cartesianChart = new CartesianChart
        {
            Series = series,
            XAxes = xAxes
        };

        Content = cartesianChart;
    }
}
