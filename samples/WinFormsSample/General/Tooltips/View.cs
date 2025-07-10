using System.Windows.Forms;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using System.Linq;

namespace WinFormsSample.General.Tooltips;

public partial class View : UserControl
{
    private readonly CartesianChart cartesianChart;
    private readonly ComboBox comboBox;

    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(100, 100);

        var values1 = new double[] { 3, 7, 3, 1, 4, 5, 6 };
        var values2 = new double[] { 2, 1, 3, 5, 3, 4, 6 };
        var positions = new TooltipPosition[]
        {
            TooltipPosition.Hidden,
            TooltipPosition.Top,
            TooltipPosition.Bottom,
            TooltipPosition.Right,
            TooltipPosition.Left,
            TooltipPosition.Center
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
            TooltipPosition = selectedPosition,
            Location = new System.Drawing.Point(0, 50),
            Size = new System.Drawing.Size(100, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };
        Controls.Add(cartesianChart);

        comboBox = new ComboBox { Location = new System.Drawing.Point(0, 0), DropDownStyle = ComboBoxStyle.DropDownList };
        comboBox.Items.AddRange([.. positions.Cast<object>()]);
        comboBox.SelectedItem = selectedPosition;
        comboBox.SelectedValueChanged += (sender, e) =>
        {
            if (comboBox.SelectedItem is TooltipPosition pos)
            {
                cartesianChart.TooltipPosition = pos;
            }
        };
        Controls.Add(comboBox);
    }
}
