using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Lines.Zoom;

namespace EtoFormsSample.Lines.Zoom;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.SeriesCollection,
            ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X // mark
        };

        Content = cartesianChart;
    }
}
