using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;

namespace WinFormsSample.Lines.Straight;

public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var values1 = new double[] { 5, 0, 5, 0, 5, 0 };
        var values2 = new double[] { 7, 2, 7, 2, 7, 2 };

        var series = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = values1,
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 0
            },
            new LineSeries<double>
            {
                Values = values2,
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 1
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
