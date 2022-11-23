using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.General.TemplatedTooltips;

namespace EtoFormsSample.General.TemplatedTooltips;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
            Tooltip = new CustomTooltip()
        };

        Content = cartesianChart;
    }
}
