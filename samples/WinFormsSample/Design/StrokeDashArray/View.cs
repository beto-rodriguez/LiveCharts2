using System.Windows.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using LiveChartsCore.SkiaSharpView.WinForms;
using SkiaSharp;

namespace WinFormsSample.Design.StrokeDashArray;

public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var values = new int[] { 4, 2, 8, 5, 3 };

        var dashedPaint = new SolidColorPaint(new SKColor(0x64, 0x95, 0xED), 10)
        {
            StrokeCap = SKStrokeCap.Round,
            PathEffect = new DashEffect([30, 20])
        };

        var series = new ISeries[]
        {
            new LineSeries<int>
            {
                Values = values,
                LineSmoothness = 1,
                GeometrySize = 22,
                Stroke = dashedPaint,
                Fill = null
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
