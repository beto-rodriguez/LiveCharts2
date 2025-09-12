using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.WinForms;

namespace WinFormsSample.Pies.Pushout;

public partial class View : UserControl
{
    private readonly PieChart pieChart;

    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        pieChart = new PieChart
        {
            Series = new[] { 6, 5, 4, 3, 2 }.AsPieSeries((value, series) =>
            {
                // pushes out the slice with the value of 6 to 30 pixels.
                if (value != 6) return;

                series.Pushout = 30;
            }),
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(pieChart);
    }
}
