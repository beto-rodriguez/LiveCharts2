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

        cartesianChart = new CartesianChart(legend: new CustomLegend())
        {
            Series = viewModel.Series,
        };

        Content = cartesianChart;
    }
}
