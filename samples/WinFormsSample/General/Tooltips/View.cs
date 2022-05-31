using System.Windows.Forms;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.General.Tooltips;

namespace WinFormsSample.General.Tooltips;

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
            LegendPosition = LegendPosition.Left,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 50),
            Size = new System.Drawing.Size(100, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);

        var b1 = new ComboBox { Text = "hidden", Location = new System.Drawing.Point(0, 0) };
        b1.Items.AddRange(new object[] { "hidden", "top", "left", "right", "bottom", "center" });
        b1.SelectedValueChanged += (object sender, System.EventArgs e) =>
        {
            if (b1.SelectedItem.ToString() == "hidden") cartesianChart.TooltipPosition = TooltipPosition.Hidden;
            if (b1.SelectedItem.ToString() == "top") cartesianChart.TooltipPosition = TooltipPosition.Top;
            if (b1.SelectedItem.ToString() == "bottom") cartesianChart.TooltipPosition = TooltipPosition.Bottom;
            if (b1.SelectedItem.ToString() == "left") cartesianChart.TooltipPosition = TooltipPosition.Left;
            if (b1.SelectedItem.ToString() == "right") cartesianChart.TooltipPosition = TooltipPosition.Right;
            if (b1.SelectedItem.ToString() == "center") cartesianChart.TooltipPosition = TooltipPosition.Center;
        };
        Controls.Add(b1);
    }
}
