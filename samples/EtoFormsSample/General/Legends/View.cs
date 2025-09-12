using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.General.Legends;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var series = new ISeries[]
        {
            new LineSeries<double> { Name = "Series 1", Values = [2, 4, 6, 8, 10] },
            new LineSeries<double> { Name = "Series 2", Values = [1, 3, 5, 7, 9] }
        };

        cartesianChart = new CartesianChart
        {
            Series = series,
            LegendPosition = LegendPosition.Right,
        };

        var b1 = new DropDown();
        b1.Items.AddRange(new ListItem[] { "hidden", "top", "left", "right", "bottom" });
        b1.SelectedValueChanged += (object sender, System.EventArgs e) =>
        {
            if (b1.SelectedKey == "hidden") cartesianChart.LegendPosition = LegendPosition.Hidden;
            if (b1.SelectedKey == "top") cartesianChart.LegendPosition = LegendPosition.Top;
            if (b1.SelectedKey == "bottom") cartesianChart.LegendPosition = LegendPosition.Bottom;
            if (b1.SelectedKey == "left") cartesianChart.LegendPosition = LegendPosition.Left;
            if (b1.SelectedKey == "right") cartesianChart.LegendPosition = LegendPosition.Right;
        };
        b1.SelectedIndex = 0;

        Content = new DynamicLayout(
            new DynamicRow(new DynamicControl { Control = b1, XScale = true }),
            cartesianChart);
    }
}
