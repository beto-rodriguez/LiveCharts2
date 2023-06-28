using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.General.VisualElements;

namespace EtoFormsSample.General.VisualElements;

public class View : Panel
{
    public View()
    {
        var viewModel = new ViewModel();

        var cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
            VisualElements = viewModel.VisualElements,
            ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X
        };

        Content = cartesianChart;
    }
}
