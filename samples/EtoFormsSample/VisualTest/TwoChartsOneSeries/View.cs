using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
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

        Content = new DynamicLayout(
            new DynamicRow(new DynamicControl() { Control = cartesianChart, YScale = true }),
            new DynamicRow(new DynamicControl() { Control = cartesianChart2, YScale = true })
            );
    }
}
