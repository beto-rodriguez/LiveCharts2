using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.StackedArea.StepArea;

public class View : Panel
{
    public View()
    {
        var values1 = new double[] { 3, 2, 3, 5, 3, 4, 6 };
        var values2 = new double[] { 6, 5, 6, 3, 8, 5, 2 };
        var values3 = new double[] { 4, 8, 2, 8, 9, 5, 3 };

        var series = new ISeries[]
        {
            new StackedStepAreaSeries<double> { Values = values1 },
            new StackedStepAreaSeries<double> { Values = values2 },
            new StackedStepAreaSeries<double> { Values = values3 }
        };

        var cartesianChart = new CartesianChart
        {
            Series = series
        };

        Content = cartesianChart;
    }
}
