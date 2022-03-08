using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Design.RadialGradients;

namespace EtoFormsSample.Design.RadialGradients;

public class View : Panel
{
    private readonly PieChart pieChart;

    public View()
    {
        var viewModel = new ViewModel();

        pieChart = new PieChart
        {
            Series = viewModel.Series,
            LegendPosition = LiveChartsCore.Measure.LegendPosition.Right,
        };

        Content = pieChart;
    }
}
