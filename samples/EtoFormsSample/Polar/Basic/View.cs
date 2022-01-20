using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
using ViewModelsSamples.Polar.Basic;

namespace EtoFormsSample.Polar.Basic;

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
        };

        Content = polarChart;
    }
}
