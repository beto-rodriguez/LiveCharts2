using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
using ViewModelsSamples.Pies.Gauge5;

namespace EtoFormsSample.Pies.Gauge5;

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
            MaxAngle = 270,
            Total = 100,
            LegendPosition = LiveChartsCore.Measure.LegendPosition.Bottom,
        };

        var b1 = new Button { Text = "Update" };
        b1.Click += (object sender, System.EventArgs e) => viewModel.DoRandomChange();

        Content = new StackLayout(b1, pieChart);
    }
}
