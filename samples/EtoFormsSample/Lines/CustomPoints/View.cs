using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Lines.CustomPoints;

namespace EtoFormsSample.Lines.CustomPoints;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
        };

        Content = cartesianChart;
    }
}
