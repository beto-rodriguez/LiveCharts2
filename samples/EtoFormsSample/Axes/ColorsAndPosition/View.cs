using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
using ViewModelsSamples.Axes.ColorsAndPosition;

namespace EtoFormsSample.Axes.ColorsAndPosition;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
            XAxes = viewModel.XAxes,
            YAxes = viewModel.YAxes,
        };

        var b1 = new Button { Text = "toggle position" };
        b1.Click += (object sender, System.EventArgs e) => viewModel.TogglePosition();

        var b2 = new Button { Text = "new color" };
        b2.Click += (object sender, System.EventArgs e) => viewModel.SetNewColor();

        var layout = new DynamicLayout(
            new DynamicRow(b1,b2),
            new DynamicRow(cartesianChart)
            );

        Content = layout;
    }
}
