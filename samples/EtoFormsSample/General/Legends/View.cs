using Eto.Forms;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
using ViewModelsSamples.General.Legends;

namespace EtoFormsSample.General.Legends;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
            LegendPosition = LegendPosition.Right,
        };

        var b1 = new ComboBox();
        b1.Items.AddRange(new ListItem[] { "hidden", "top", "left", "right", "bottom" });
        b1.SelectedValueChanged += (object sender, System.EventArgs e) =>
        {
            if (b1.SelectedKey == "hidden") cartesianChart.LegendPosition = LegendPosition.Hidden;
            if (b1.SelectedKey == "top") cartesianChart.LegendPosition = LegendPosition.Top;
            if (b1.SelectedKey == "bottom") cartesianChart.LegendPosition = LegendPosition.Bottom;
            if (b1.SelectedKey == "left") cartesianChart.LegendPosition = LegendPosition.Left;
            if (b1.SelectedKey == "right") cartesianChart.LegendPosition = LegendPosition.Right;
        };

        Content = new StackLayout(b1, cartesianChart);
    }
}
