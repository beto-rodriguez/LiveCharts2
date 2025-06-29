using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace WinFormsSample.StepLines.Basic;

public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var values = new double[] { 2, 1, 3, 4, 3, 4, 6 };

        var series = new ISeries[]
        {
            new StepLineSeries<double>
            {
                Values = values,
                Fill = null,
                GeometrySize = 20
            }
        };

        var title = new DrawnLabelVisual(
            new LabelGeometry
            {
                Text = "My chart title",
                Paint = new SolidColorPaint(SKColor.Parse("#303030")),
                TextSize = 25,
                Padding = new LiveChartsCore.Drawing.Padding(15)
            });

        var cartesianChart = new CartesianChart
        {
            Series = series,
            Title = title,
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);
    }
}
