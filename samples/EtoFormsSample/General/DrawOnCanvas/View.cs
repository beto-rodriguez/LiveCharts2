using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.General.DrawOnCanvas;

namespace EtoFormsSample.General.DrawOnCanvas;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart();
        cartesianChart.UpdateStarted += chart =>
        {
            viewModel.ChartUpdated(new(chart));
        };

        Content = cartesianChart;
    }
}
