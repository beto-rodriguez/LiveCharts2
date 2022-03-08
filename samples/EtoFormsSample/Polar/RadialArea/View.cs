using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Polar.RadialArea;

namespace EtoFormsSample.Polar.RadialArea;

public class View : Panel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    public View()
    {
        var viewModel = new ViewModel();

        var polarChart = new PolarChart
        {
            Series = viewModel.Series,
            AngleAxes = viewModel.AngleAxes,
            InitialRotation = -45,
        };

        Content = polarChart;
    }
}
