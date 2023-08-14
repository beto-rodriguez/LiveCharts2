using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Pies.Gauge4;

namespace EtoFormsSample.Pies.Gauge4;

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
            MaxAngle = 350,
            MinValue = 0,
            MaxValue = 100,
        };

        Content = pieChart;
    }
}
