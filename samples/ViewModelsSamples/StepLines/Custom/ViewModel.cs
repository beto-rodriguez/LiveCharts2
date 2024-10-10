using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace ViewModelsSamples.StepLines.Custom;

public partial class ViewModel : ObservableObject
{
    public ISeries[] Series { get; set; } =
    [
        new StepLineSeries<double>
        {
            Values = [2, 1, 4, 2, 2, -5, -2],
            Fill = null,
            GeometrySize = 20
        },

        // use the second type parameter to specify the geometry to draw for every point
        // there are already many predefined geometries in the
        // LiveChartsCore.SkiaSharpView.Drawing.Geometries namespace
        new StepLineSeries<double, RectangleGeometry>
        {
            Values = [3, 3, -3, -2, -4, -3, -1],
            Fill = null,
            GeometrySize = 20
        },

        // You can also use SVG paths to draw the geometry
        // LiveCharts already provides some predefined paths in the SVGPoints class.
        new StepLineSeries<double, VariableSVGPathGeometry>
        {
            Values = [-2, 2, 1, 3, -1, 4, 3],
            Fill = null,
            GeometrySvg = SVGPoints.Star,
            GeometrySize = 20
        },

        // you can declare your own gemetry and use the SkiaSharp api to draw it
        new StepLineSeries<double, MyGeometry>
        {
            Values = [4, 5, 2, 4, 3, 2, 1],
            Fill = null,
            GeometrySize = 20
        },
    ];
}
