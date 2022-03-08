using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Axes.Paging;

namespace EtoFormsSample.Axes.Paging;

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
            ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X,
        };

        var b1 = new Button { Text = "Go to page 1" };
        b1.Click += (object sender, System.EventArgs e) => viewModel.GoToPage1();

        var b2 = new Button { Text = "Go to page 2" };
        b2.Click += (object sender, System.EventArgs e) => viewModel.GoToPage2();

        var b3 = new Button { Text = "Go to page 3" };
        b3.Click += (object sender, System.EventArgs e) => viewModel.GoToPage3();

        var b4 = new Button { Text = "Clear" };
        b4.Click += (object sender, System.EventArgs e) => viewModel.SeeAll();

        var buttons = new StackLayout(b1, b2, b3, b4) { Orientation = Orientation.Horizontal, Padding = 2, Spacing = 4 };

        Content = new DynamicLayout(buttons, cartesianChart);
    }
}
