using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Pies.AngularGauge;

namespace EtoFormsSample.Pies.AngularGauge;

public class View : Panel
{
    private readonly PieChart pieChart;

    public View()
    {
        var viewModel = new ViewModel();

        pieChart = new PieChart
        {
            Series = viewModel.Series,
            VisualElements = viewModel.VisualElements,
            InitialRotation = -225,
            MaxAngle = 270,
            MinValue = 0,
            MaxValue = 100
        };

        var b1 = new Button { Text = "Update" };
        b1.Click += (object sender, System.EventArgs e) => viewModel.DoRandomChange();

        Content = new DynamicLayout(new StackLayout(b1), pieChart);
    }
}
