using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
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
            LegendFont = Eto.Drawing.Fonts.Monospace(25),
            LegendTextColor = Eto.Drawing.Color.FromArgb(50, 50, 50),
            LegendBackColor = Eto.Drawing.Color.FromArgb(250, 250, 250),
        };

        Content = cartesianChart;
    }
}
