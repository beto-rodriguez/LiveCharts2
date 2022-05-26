using Eto.Forms;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.General.Tooltips;

namespace EtoFormsSample.General.Tooltips;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
            LegendPosition = LegendPosition.Left
        };

        var b1 = new DropDown();
        b1.Items.AddRange(new ListItem[] { "hidden", "top", "left", "right", "bottom", "center" });
        b1.SelectedValueChanged += (object sender, System.EventArgs e) =>
        {
            if (b1.SelectedKey == "hidden") cartesianChart.TooltipPosition = TooltipPosition.Hidden;
            if (b1.SelectedKey == "top") cartesianChart.TooltipPosition = TooltipPosition.Top;
            if (b1.SelectedKey == "bottom") cartesianChart.TooltipPosition = TooltipPosition.Bottom;
            if (b1.SelectedKey == "left") cartesianChart.TooltipPosition = TooltipPosition.Left;
            if (b1.SelectedKey == "right") cartesianChart.TooltipPosition = TooltipPosition.Right;
            if (b1.SelectedKey == "center") cartesianChart.TooltipPosition = TooltipPosition.Center;
        };
        b1.SelectedIndex = 0;

        Content = new DynamicLayout(
            new DynamicRow(new DynamicControl() { Control = b1, XScale = true }),
            cartesianChart);
    }
}
