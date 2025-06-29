using System.Linq;
using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.General.Tooltips;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var values1 = new double[] { 3, 7, 3, 1, 4, 5, 6 };
        var values2 = new double[] { 2, 1, 3, 5, 3, 4, 6 };
        var positions = new[]
        {
            "hidden",
            "top",
            "bottom",
            "right",
            "left",
            "center"
        };
        var selectedPosition = TooltipPosition.Top;

        var series = new ISeries[]
        {
            new ColumnSeries<double> { Values = values1, Name = "Sales" },
            new LineSeries<double> { Values = values2, Name = "Customers" }
        };

        cartesianChart = new CartesianChart
        {
            Series = series,
            TooltipPosition = selectedPosition
        };

        var b1 = new DropDown();
        b1.Items.AddRange(positions.Select(x => new ListItem(x)));
        b1.SelectedValueChanged += (sender, e) =>
        {
            if (b1.SelectedKey == "hidden") cartesianChart.TooltipPosition = TooltipPosition.Hidden;
            if (b1.SelectedKey == "top") cartesianChart.TooltipPosition = TooltipPosition.Top;
            if (b1.SelectedKey == "bottom") cartesianChart.TooltipPosition = TooltipPosition.Bottom;
            if (b1.SelectedKey == "left") cartesianChart.TooltipPosition = TooltipPosition.Left;
            if (b1.SelectedKey == "right") cartesianChart.TooltipPosition = TooltipPosition.Right;
            if (b1.SelectedKey == "center") cartesianChart.TooltipPosition = TooltipPosition.Center;
        };
        b1.SelectedIndex = 1; // Top

        Content = new DynamicLayout(
            new DynamicRow(new DynamicControl { Control = b1, XScale = true }),
            cartesianChart);
    }

    public class ListItem(string value) : IListItem
    {
        public string Text { get => value; set => throw new System.NotImplementedException(); }

        public string Key => value;
    }
}
