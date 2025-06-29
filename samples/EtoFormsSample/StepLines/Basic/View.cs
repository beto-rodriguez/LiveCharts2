using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace EtoFormsSample.StepLines.Basic;

public class View : Panel
{
    public View()
    {
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
            Title = title
        };

        Content = cartesianChart;
    }
}
