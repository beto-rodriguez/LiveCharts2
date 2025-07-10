using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace WinFormsSample.Lines.Padding;

public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var values = new double[] { 2, 1, 3, 5, 3, 4, 6 };

        var series = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = values,
                DataPadding = new LiveChartsCore.Drawing.LvcPoint(0, 0),
                GeometryStroke = null,
                GeometryFill = null
            }
        };

        var drawMarginFrame = new DrawMarginFrame
        {
            Stroke = new SolidColorPaint(SKColor.Parse("#64b4b4b4")),
            Fill = new SolidColorPaint(SKColor.Parse("#32dcdcdc"))
        };

        var cartesianChart = new CartesianChart
        {
            Series = series,
            DrawMarginFrame = drawMarginFrame,
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);
    }
}
