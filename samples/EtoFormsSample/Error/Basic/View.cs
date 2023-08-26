using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Error.Basic;

namespace EtoFormsSample.Error.Basic;

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
            Series = viewModel.Series0
        };

        var cartesianChart2 = new CartesianChart
        {
            Series = viewModel.Series1
        };

        var cartesianChart3 = new CartesianChart
        {
            Series = viewModel.Series1,
            XAxes = viewModel.DateTimeAxis
        };

        Content = new DynamicLayout(
            new DynamicRow(new DynamicControl() { Control = cartesianChart, YScale = true }),
            new DynamicRow(new DynamicControl() { Control = cartesianChart2, YScale = true }),
            new DynamicRow(new DynamicControl() { Control = cartesianChart3, YScale = true })
            );

        Content = cartesianChart;
    }
}
