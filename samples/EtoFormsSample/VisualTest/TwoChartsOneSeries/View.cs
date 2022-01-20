using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
using ViewModelsSamples.VisualTest.TwoChartsOneSeries;

namespace EtoFormsSample.VisualTest.TwoChartsOneSeries;

public class View : Panel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    public View()
    {
        var viewModel = new ViewModel();

        var cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
            ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X,
        };

        var cartesianChart2 = new CartesianChart
        {
            Series = viewModel.Series,
            ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X,
        };

        var splitContainer = new Splitter
        {
            Orientation = Orientation.Horizontal
        };

        splitContainer.Panel1 = cartesianChart;
        splitContainer.Panel2 = cartesianChart2;
        Content = splitContainer;
    }
}
