using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;

namespace WinFormsSample.StackedBars.Groups;

public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var values1 = new int[] { 3, 5, 3 };
        var values2 = new int[] { 4, 2, 3 };
        var values3 = new int[] { 4, 6, 6 };
        var values4 = new int[] { 2, 5, 4 };
        var labels = new string[] { "Category 1", "Category 2", "Category 3" };

        var series = new ISeries[]
        {
            new StackedColumnSeries<int> { Values = values1, StackGroup = 0 },
            new StackedColumnSeries<int> { Values = values2, StackGroup = 0 },
            new StackedColumnSeries<int> { Values = values3, StackGroup = 1 },
            new StackedColumnSeries<int> { Values = values4, StackGroup = 1 }
        };

        var xAxes = new Axis[]
        {
            new() {
                LabelsRotation = -15,
                Labels = labels,
                MinStep = 1,
                ForceStepToMin = true
            }
        };

        var cartesianChart = new CartesianChart
        {
            Series = series,
            XAxes = xAxes,
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);
    }
}
