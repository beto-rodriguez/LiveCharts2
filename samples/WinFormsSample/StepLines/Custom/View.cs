using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using SkiaSharp;
using LiveChartsCore.Drawing;
using ViewModelsSamples.StepLines.Custom;

namespace WinFormsSample.StepLines.Custom;

public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var values1 = new double[] { 2, 1, 4, 2, 2, -5, -2 };
        var values2 = new double[] { 3, 3, -3, -2, -4, -3, -1 };
        var values3 = new double[] { -2, 2, 1, 3, -1, 4, 3 };
        var values4 = new double[] { 4, 5, 2, 4, 3, 2, 1 };
        var pinPath = SVGPoints.Pin;

        var series = new ISeries[]
        {
            new StepLineSeries<double>
            {
                Values = values1,
                Fill = null,
                GeometrySize = 20
            },
            new StepLineSeries<double, StarGeometry>
            {
                Values = values2,
                Fill = null,
                GeometrySize = 20
            },
            new StepLineSeries<double, VariableSVGPathGeometry>
            {
                Values = values3,
                GeometrySvg = pinPath,
                Fill = null,
                GeometrySize = 20
            },
            new StepLineSeries<double, MyGeometry>
            {
                Values = values4,
                Fill = null,
                GeometrySize = 20
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
