using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Pies.Gauge2;

namespace EtoFormsSample.Pies.Gauge2;

public class View : Panel
{
    private readonly PieChart pieChart;

    public View()
    {
        var viewModel = new ViewModel();

        pieChart = new PieChart
        {
            Series = viewModel.Series,
            InitialRotation = -225,
            MaxAngle = 270,
            MinValue = 0,
            MaxValue = 100,
        };

        Content = pieChart;
    }
}
