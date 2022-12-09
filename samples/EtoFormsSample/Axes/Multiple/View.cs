using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Axes.Multiple;

namespace EtoFormsSample.Axes.Multiple;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
            YAxes = viewModel.YAxes,
            LegendPosition = LiveChartsCore.Measure.LegendPosition.Left,
            LegendTextPaint = viewModel.LegendTextPaint,
            LegendBackgroundPaint = viewModel.LedgendBackgroundPaint,
            LegendTextSize = 16
        };

        Content = cartesianChart;
    }
}
