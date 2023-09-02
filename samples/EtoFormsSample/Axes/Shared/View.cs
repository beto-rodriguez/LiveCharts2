using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Axes.Shared;

namespace EtoFormsSample.Axes.Shared;

public class View : Panel
{
    public View()
    {
        var viewModel = new ViewModel();

        var cartesianChart = new CartesianChart
        {
            Series = viewModel.SeriesCollection1,
            ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X,
            DrawMargin = viewModel.DrawMargin,
            XAxes = viewModel.X1
        };

        var cartesianChart2 = new CartesianChart
        {
            Series = viewModel.SeriesCollection2,
            ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X,
            DrawMargin = viewModel.DrawMargin,
            XAxes = viewModel.X2
        };

        Content = new DynamicLayout(
            new DynamicRow(new DynamicControl() { Control = cartesianChart, YScale = true }),
            new DynamicRow(new DynamicControl() { Control = cartesianChart2, YScale = true }));
    }
}
