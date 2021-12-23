using System.Windows.Forms;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.General.Legends;

namespace WinFormsSample.General.Legends;

public partial class View : UserControl
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(100, 100);

        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
            LegendPosition = LegendPosition.Right,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 50),
            Size = new System.Drawing.Size(100, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);

        var b1 = new ComboBox { Text = "hidden", Location = new System.Drawing.Point(0, 0) };
        b1.Items.AddRange(new object[] { "hidden", "top", "left", "right", "bottom" });
        b1.SelectedValueChanged += (object sender, System.EventArgs e) =>
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
