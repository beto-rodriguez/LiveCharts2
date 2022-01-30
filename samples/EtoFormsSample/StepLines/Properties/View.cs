using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.StepLines.Properties;

namespace EtoFormsSample.StepLines.Properties;

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

        Content = cartesianChart;

        var b1 = new Button { Text = "new values" };
        b1.Click += (object sender, System.EventArgs e) => viewModel.ChangeValuesInstance();

        var b2 = new Button { Text = "new fill" };
        b2.Click += (object sender, System.EventArgs e) => viewModel.NewFill();

        var b3 = new Button { Text = "new stroke" };
        b3.Click += (object sender, System.EventArgs e) => viewModel.NewStroke();

        var b4 = new Button { Text = "newGfill" };
        b4.Click += (object sender, System.EventArgs e) => viewModel.NewGeometryFill();

        var b5 = new Button { Text = "newGstroke" };
        b5.Click += (object sender, System.EventArgs e) => viewModel.NewGeometryStroke();

        var b8 = new Button { Text = "+ size" };
        b8.Click += (object sender, System.EventArgs e) => viewModel.IncreaseGeometrySize();

        var b9 = new Button { Text = "- size" };
        b9.Click += (object sender, System.EventArgs e) => viewModel.DecreaseGeometrySize();

        var b10 = new Button { Text = "new series" };
        b10.Click += (object sender, System.EventArgs e) =>
        {
            viewModel.ChangeSeriesInstance();
            cartesianChart.Series = viewModel.Series;
        };

        var buttons = new StackLayout(b1, b2, b3, b4, b5, b8, b9, b10) { Orientation = Orientation.Horizontal, Padding = 2, Spacing = 4 };

        Content = new DynamicLayout(buttons, cartesianChart);
    }
}
