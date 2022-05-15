using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.General.Visibility;

namespace EtoFormsSample.General.Visibility;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
        };

        var b1 = new Button { Text = "toggle 1" };
        b1.Click += (object sender, System.EventArgs e) => viewModel.ToggleSeries0();

        var b2 = new Button { Text = "toggle 2" };
        b2.Click += (object sender, System.EventArgs e) => viewModel.ToggleSeries1();

        var b3 = new Button { Text = "toggle 3" };
        b3.Click += (object sender, System.EventArgs e) => viewModel.ToggleSeries2();

        var buttons = new StackLayout(b1, b2, b3) { Orientation = Orientation.Horizontal, Padding = 2, Spacing = 4 };

        Content = new DynamicLayout(buttons, cartesianChart);
    }
}
