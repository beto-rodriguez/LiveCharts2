using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Design.LinearGradients;

namespace EtoFormsSample.Design.LinearGradients;

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
        };

        Content = cartesianChart;
    }
}
