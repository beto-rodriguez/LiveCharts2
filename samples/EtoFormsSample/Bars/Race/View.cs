using System.Threading.Tasks;
using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
using ViewModelsSamples.Bars.Race;

namespace EtoFormsSample.Bars.Race;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;
    private readonly ViewModel viewModel;

    public View()
    {
        viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
        };

        Content = cartesianChart;

        UpdateViewModel();
    }

    public async void UpdateViewModel()
    {
        while (true)
        {
            viewModel.RandomIncrement();
            cartesianChart.Series = viewModel.Series;
            await Task.Delay(1500);
        }
    }
}
