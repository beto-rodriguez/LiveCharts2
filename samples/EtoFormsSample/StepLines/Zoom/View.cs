using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.StepLines.Zoom;

namespace EtoFormsSample.StepLines.Zoom;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.SeriesCollection,
            ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X,
        };

        Content = cartesianChart;
    }
}
