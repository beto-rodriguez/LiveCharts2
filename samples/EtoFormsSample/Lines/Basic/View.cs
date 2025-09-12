using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Eto;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;

namespace EtoFormsSample.Lines.Basic;

public class View : Panel
{
    public View()
    {
        var values1 = new double[] { 2, 1, 3, 5, 3, 4, 6 };
        var values2 = new int[] { 4, 2, 5, 2, 4, 5, 3 };

        var series = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = values1,
                Fill = null,
                GeometrySize = 20
            },
            new LineSeries<int, StarGeometry>
            {
                Values = values2,
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
            Title = title
        };

        Content = cartesianChart;
    }
}
