using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.General.TemplatedTooltips;

namespace EtoFormsSample.General.TemplatedTooltips;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var viewModel = new ViewModel();

        var series = new ISeries[]
        {
            new ColumnSeries<GeometryPoint> { Values = viewModel.Values }
        };

        cartesianChart = new CartesianChart
        {
            Series = series,
            Tooltip = new CustomTooltip()
        };

        Content = cartesianChart;
    }
}
