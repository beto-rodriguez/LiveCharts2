using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Pies.Gauge1;

namespace EtoFormsSample.Pies.Gauge1;

public class View : Panel
{
    private readonly PieChart pieChart;

    public View()
    {
        var viewModel = new ViewModel();

        pieChart = new PieChart
        {
            Series = viewModel.Series,
            InitialRotation = -90,
            MinValue = 0,
            MaxValue = 100,
        };

        Content = pieChart;
    }
}
