using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Lines.Custom;

public partial class ViewModel : ObservableObject
{
    public ViewModel()
    {
        Series = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = new double[] { 3, 1, 4, 3, 2, -5, -2 },
                GeometrySize = 10,
                Fill = null
            },

            // use the second argument type to specify the geometry to draw for every point
            // there are already many predefined geometries in the
            // LiveChartsCore.SkiaSharpView.Drawing.Geometries namespace
            new LineSeries<double, RectangleGeometry>
            {
                Values = new double[] { 3, 3, -3, -2, -4, -3, -1 },
                Fill = null
            },

            // You can also use a SVG path data to draw the geometry
            new LineSeries<double, SVGPathGeometry>
            {
                Values = new double[] { -2, 2, 1, 3, -1, 4, 3 },
                Stroke = new SolidColorPaint(SKColors.DarkOliveGreen, 3),
                Fill = null,
                GeometrySvg = SVGPoints.Star,
                GeometryStroke = new SolidColorPaint(SKColors.DarkOliveGreen, 3),
                GeometryFill = new SolidColorPaint(SKColors.White),
                GeometrySize = 35
            },

            new LineSeries<double, SVGPathGeometry>
            {
                Values = new double[] { 3, 5, 2, 4, 3, 2, 1 },
                Stroke = new SolidColorPaint(SKColors.DarkOliveGreen, 3),
                Fill = null,
                GeometrySvg = SVGPoints.Pin,
                GeometryStroke = new SolidColorPaint(SKColors.DarkOliveGreen, 3),
                GeometryFill = new SolidColorPaint(SKColors.White),
                GeometrySize = 35
            },
        };

        var s = (LineSeries<double, SVGPathGeometry>)Series[2];

        s.ChartPointPointerDown += (chart, point) =>
        {
            if (point is null) return;
            // you can change the svg at runtime
            point.Context.Series.GeometrySvg =
                s.GeometrySvg == SVGPoints.Star ? SVGPoints.Gem : SVGPoints.Star;
        };
    }

    public ISeries[] Series { get; set; }
}
