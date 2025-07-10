using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.WinForms;

namespace WinFormsSample.Pies.NightingaleRose;

public partial class View : UserControl
{
    private readonly PieChart pieChart;

    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var outer = 0;
        var data = new[] { 6, 5, 4, 3, 2 };

        // you can convert any array, list or IEnumerable<T> to a pie series collection:
        var seriesCollection = data.AsPieSeries((value, series) =>
        {
            // this method is called once per element in the array
            // we are decrementing the outer radius 50px
            // on every element in the array.

            series.InnerRadius = 50;
            series.OuterRadiusOffset = outer;
            outer += 50;
        });

        pieChart = new PieChart
        {
            Series = seriesCollection,
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(pieChart);
    }
}
