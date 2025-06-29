using System.Windows.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinForms;

namespace WinFormsSample.Bars.Layered;

public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var values1 = new int[] { 6, 3, 5, 7, 3, 4, 6, 3 };
        var values2 = new int[] { 2, 4, 8, 9, 5, 2, 4, 7 };

        var series = new ISeries[]
        {
            new ColumnSeries<int>
            {
                Values = values1,
                MaxBarWidth = 999999,
                IgnoresBarPosition = true
            },
            new ColumnSeries<int>
            {
                Values = values2,
                MaxBarWidth = 30,
                IgnoresBarPosition = true
            }
        };

        var cartesianChart = new CartesianChart
        {
            Series = series,
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);
    }
}
