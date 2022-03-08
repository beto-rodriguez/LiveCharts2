using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.General.MultiThreading;

namespace EtoFormsSample.General.MultiThreading;

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
        };

        Content = cartesianChart;
    }
}
