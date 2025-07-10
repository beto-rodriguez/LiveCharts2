using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using SkiaSharp;

namespace EtoFormsSample.Bars.Custom;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var values1 = new double[] { 2, 1, 4 };
        var values2 = new double[] { 4, 3, 6 };
        var values3 = new double[] { -2, 2, 1 };
        var values4 = new double[] { 1, 2, 3 };
        var starPath = LiveChartsCore.Drawing.SVGPoints.Star;

        var series = new ISeries[]
        {
            new ColumnSeries<double> { Values = values1 },
            new ColumnSeries<double, DiamondGeometry> { Values = values2 },
            new ColumnSeries<double, VariableSVGPathGeometry> { Values = values3, GeometrySvg = starPath },
            new ColumnSeries<double, ViewModelsSamples.Bars.Custom.MyGeometry> { Values = values4 }
        };

        cartesianChart = new CartesianChart
        {
            Series = series,
        };

        Content = cartesianChart;
    }
}
