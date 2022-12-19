using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Axes.NamedLabels;

namespace EtoFormsSample.Axes.NamedLabels;

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
            TooltipPosition = LiveChartsCore.Measure.TooltipPosition.Left, // mark
            TooltipTextPaint = viewModel.TooltipTextPaint, // mark
            TooltipBackgroundPaint = viewModel.TooltipBackgroundPaint, // mark
            TooltipTextSize = 16
        };

        Content = cartesianChart;
    }
}
