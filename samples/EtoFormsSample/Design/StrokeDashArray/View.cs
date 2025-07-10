using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using SkiaSharp;

namespace EtoFormsSample.Design.StrokeDashArray;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var values = new int[] { 4, 2, 8, 5, 3 };

        var dashedPaint = new SolidColorPaint(new SKColor(0x64, 0x95, 0xED), 10)
        {
            StrokeCap = SKStrokeCap.Round,
            PathEffect = new DashEffect(new float[] { 30, 20 })
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

        cartesianChart = new CartesianChart
        {
            Series = series,
        };

        Content = cartesianChart;
    }
}
