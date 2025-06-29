using System.Windows.Forms;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinForms;

namespace WinFormsSample.General.Legends;

public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(400, 350);

        var series = new ISeries[]
        {
            new LineSeries<double> { Name = "Series 1", Values = [2, 4, 6, 8, 10] },
            new LineSeries<double> { Name = "Series 2", Values = [1, 3, 5, 7, 9] }
        };

        var cartesianChart = new CartesianChart
        {
            Series = series,
            LegendPosition = LegendPosition.Right,
            Location = new System.Drawing.Point(0, 50),
            Size = new System.Drawing.Size(400, 300),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);

        var b1 = new ComboBox { Text = "hidden", Location = new System.Drawing.Point(0, 0), Width = 120 };
        b1.Items.AddRange(["hidden", "top", "left", "right", "bottom"]);
        b1.SelectedValueChanged += (sender, e) =>
        {
            if (b1.SelectedItem.ToString() == "hidden") cartesianChart.LegendPosition = LegendPosition.Hidden;
            if (b1.SelectedItem.ToString() == "top") cartesianChart.LegendPosition = LegendPosition.Top;
            if (b1.SelectedItem.ToString() == "bottom") cartesianChart.LegendPosition = LegendPosition.Bottom;
            if (b1.SelectedItem.ToString() == "left") cartesianChart.LegendPosition = LegendPosition.Left;
            if (b1.SelectedItem.ToString() == "right") cartesianChart.LegendPosition = LegendPosition.Right;
        };
        Controls.Add(b1);
    }
}
