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
            TooltipFont = Eto.Drawing.Fonts.Monospace(25), // mark
            TooltipTextColor = Eto.Drawing.Color.FromArgb(242, 244, 195), // mark
            TooltipBackColor = Eto.Drawing.Color.FromArgb(72, 0, 50), // mark
        };

        Content = cartesianChart;
    }
}
