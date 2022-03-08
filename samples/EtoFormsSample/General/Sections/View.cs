using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.General.Sections;

namespace EtoFormsSample.General.Sections;

public class View : Panel
{
    public View()
    {
        var viewModel = new ViewModel();

        var cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
            Sections = viewModel.Sections,
        };

        Content = cartesianChart;
    }
}
