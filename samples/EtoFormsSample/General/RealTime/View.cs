using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.General.RealTime;

namespace EtoFormsSample.General.RealTime;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            SyncContext = viewModel.Sync,
            Series = viewModel.Series,
            XAxes = viewModel.XAxes
        };

        Content = cartesianChart;
    }
}
