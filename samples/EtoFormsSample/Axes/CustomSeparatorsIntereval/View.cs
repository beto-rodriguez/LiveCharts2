using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Axes.CustomSeparatorsInterval;

namespace EtoFormsSample.Axes.CustomSeparatorsInterval;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
            YAxes = viewModel.YAxes
        };

        Content = cartesianChart;
    }
}
