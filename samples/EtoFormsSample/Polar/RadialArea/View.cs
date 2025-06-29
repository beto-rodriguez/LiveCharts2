using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace EtoFormsSample.Polar.RadialArea;

public class View : Panel
{
    public View()
    {
        var values1 = new int[] { 7, 5, 7, 5, 6 };
        var values2 = new int[] { 2, 7, 5, 9, 7 };
        var labels = new string[] { "first", "second", "third", "forth", "fifth" };
        var tangentAngle = LiveCharts.TangentAngle;

        var series = new ISeries[]
        {
            new PolarLineSeries<int>
            {
                Values = values1,
                LineSmoothness = 0,
                GeometrySize = 0,
                Fill = new SolidColorPaint(SKColor.Parse("#900000ff"))
            },
            new PolarLineSeries<int>
            {
                Values = values2,
                LineSmoothness = 1,
                GeometrySize = 0,
                Fill = new SolidColorPaint(SKColor.Parse("#90ff0000"))
            }
        };

        var angleAxes = new PolarAxis[]
        {
            new() {
                LabelsRotation = tangentAngle,
                Labels = labels
            }
        };

        var polarChart = new PolarChart
        {
            Series = series,
            AngleAxes = angleAxes,
            InitialRotation = -45
        };

        Content = polarChart;
    }
}
