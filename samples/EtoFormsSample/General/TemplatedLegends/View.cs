using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.General.TemplatedLegends;

namespace EtoFormsSample.General.TemplatedLegends;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
            LegendPosition = LiveChartsCore.Measure.LegendPosition.Right,
            Legend = new CustomLegend()
        };

        Content = cartesianChart;
    }
}
