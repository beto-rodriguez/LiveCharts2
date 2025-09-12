using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.General.Visibility;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;
    private readonly ISeries[] series;

    public View()
    {
        var values0 = new double[] { 2, 5, 4, 3 };
        var values1 = new double[] { 1, 2, 3, 4 };
        var values2 = new double[] { 4, 3, 2, 1 };
        var isVisible = new bool[] { true, true, true };

        series =
        [
            new ColumnSeries<double> { Values = values0, IsVisible = isVisible[0] },
            new ColumnSeries<double> { Values = values1, IsVisible = isVisible[1] },
            new ColumnSeries<double> { Values = values2, IsVisible = isVisible[2] }
        ];

        cartesianChart = new CartesianChart
        {
            Series = series,
        };

        var b1 = new Button { Text = "toggle 1" };
        b1.Click += (sender, e) => ToggleSeries(0);

        var b2 = new Button { Text = "toggle 2" };
        b2.Click += (sender, e) => ToggleSeries(1);

        var b3 = new Button { Text = "toggle 3" };
        b3.Click += (sender, e) => ToggleSeries(2);

        var buttons = new StackLayout(b1, b2, b3) { Orientation = Orientation.Horizontal, Padding = 2, Spacing = 4 };

        Content = new DynamicLayout(buttons, cartesianChart);
    }

    private void ToggleSeries(int index)
    {
        if (series[index] is ISeries s)
        {
            s.IsVisible = !s.IsVisible;
        }
    }
}
