using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.General.ConditionalDraw;

namespace EtoFormsSample.General.ConditionalDraw;

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
            Sections = viewModel.Sections,
            YAxes = viewModel.Y,
            LegendPosition = LiveChartsCore.Measure.LegendPosition.Right,
        };

        Content = cartesianChart;
    }
}
