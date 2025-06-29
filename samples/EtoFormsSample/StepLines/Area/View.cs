using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace EtoFormsSample.StepLines.Area;

public class View : Panel
{
    public View()
    {
        var values = new double[] { -2, -1, 3, 5, 3, 4, 6 };

        var series = new ISeries[]
        {
            new StepLineSeries<double>
            {
                Values = values,
                Fill = new SolidColorPaint(SKColor.Parse("#6495ED")),
                Stroke = null,
                GeometryFill = null,
                GeometryStroke = null
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
            DrawMarginFrame = drawMarginFrame
        };

        Content = cartesianChart;
    }
}
