using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
using ViewModelsSamples.Axes.Style;

namespace EtoFormsSample.Axes.Style;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
            XAxes = viewModel.XAxes,
            YAxes = viewModel.YAxes,
            DrawMarginFrame = viewModel.Frame,
        };

        Content = cartesianChart;
    }
}
