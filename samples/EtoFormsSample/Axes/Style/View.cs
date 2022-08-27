using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Axes.Style;

namespace EtoFormsSample.Axes.Style;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        BackgroundColor = new Eto.Drawing.Color(60, 60, 60);
        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
            XAxes = viewModel.XAxes,
            YAxes = viewModel.YAxes,
            DrawMarginFrame = viewModel.Frame,
            ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.Both,
            TooltipPosition = LiveChartsCore.Measure.TooltipPosition.Hidden
        };

        Content = cartesianChart;
    }
}
