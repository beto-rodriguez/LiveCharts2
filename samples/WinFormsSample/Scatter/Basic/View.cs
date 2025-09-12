using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.Defaults;

namespace WinFormsSample.Scatter.Basic;

public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var values = new ObservablePoint[]
        {
            new(2.2, 5.4),
            new(3.6, 9.6),
            new(9.9, 5.2),
            new(8.1, 4.7),
            new(5.3, 7.1)
        };

        var series = new ISeries[]
        {
            new ScatterSeries<ObservablePoint>
            {
                Values = values
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
