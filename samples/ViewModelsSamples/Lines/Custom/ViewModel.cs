using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace ViewModelsSamples.Lines.Custom;

public class ViewModel
{
    public ISeries[] Series { get; set; } =
    [
        new LineSeries<double>
        {
            Values = [2, 1, 4, 2, 2, -5, -2],
            Fill = null,
            GeometrySize = 20
        },

        // use the second generic parameter to define the geometry to draw
        // there are many predefined geometries in the LiveChartsCore.Drawing namespace
        // for example, the StarGeometry, CrossGeometry, RectangleGeometry and DiamondGeometry
        new LineSeries<double, StarGeometry>
        {
            Values = [3, 3, -3, -2, -4, -3, -1],
            Fill = null,
            GeometrySize = 20
        },

        // You can also use SVG paths to draw the geometry
        // the VariableSVGPathGeometry can change the drawn path at runtime
        new LineSeries<double, VariableSVGPathGeometry>
        {
            Values = [-2, 2, 1, 3, -1, 4, 3],
            Fill = null,
            GeometrySvg = SVGPoints.Pin,
            GeometrySize = 20
        },

        // finally you can also use SkiaSharp to draw your own geometry
        new LineSeries<double, MyGeometry>
        {
            Values = [4, 5, 2, 4, 3, 2, 1],
            Fill = null,
            GeometrySize = 20
        },
    ];
}
