using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;

namespace WinFormsSample.StepLines.Zoom;

public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var values = Fetch();

        var series = new ISeries[]
        {
            new StepLineSeries<int>
            {
                Values = values
            }
        };

        var cartesianChart = new CartesianChart
        {
            Series = series,
            ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X,
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);
    }

    private static int[] Fetch()
    {
        var values = new int[100];
        var r = new System.Random();
        var t = 0;
        for (var i = 0; i < 100; i++)
        {
            t += r.Next(-90, 100);
            values[i] = t;
        }
        return values;
    }
}
