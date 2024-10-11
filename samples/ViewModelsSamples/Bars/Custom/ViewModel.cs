using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace ViewModelsSamples.Bars.Custom;

public class ViewModel
{
    public ISeries[] Series { get; set; } = [
        new ColumnSeries<double> ([2, 1, 4]),

        // use the second generic parameter to define the geometry to draw
        // there are many predefined geometries in the LiveChartsCore.Drawing namespace
        // for example, the StarGeometry, CrossGeometry, RectangleGeometry and DiamondGeometry
        new ColumnSeries<double, DiamondGeometry>([4, 3, 6]),

        // You can also use SVG paths to draw the geometry
        // the VariableSVGPathGeometry can change the drawn path at runtime
        new ColumnSeries<double, VariableSVGPathGeometry>([-2, 2, 1])
        {
            GeometrySvg = SVGPoints.Star
        },

        // finally you can also use SkiaSharp to draw your own geometry
        new ColumnSeries<double, MyGeometry>([4, 5, 2])
    ];
}
