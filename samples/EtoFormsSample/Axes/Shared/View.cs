using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
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
            XAxes = viewModel.SharedXAxis, // <-- notice we are using the same variable for both charts, this syncs both charts
        };

        var cartesianChart2 = new CartesianChart
        {
            Series = viewModel.SeriesCollection2,
            ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X,
            XAxes = viewModel.SharedXAxis, // <-- notice we are using the same variable for both charts, this syncs both charts
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
