using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Lines.Custom;

public partial class ViewModel : ObservableObject
{
    public ISeries[] Series { get; set; } =
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
        new LineSeries<double, LiveChartsCore.SkiaSharpView.Drawing.Geometries.RectangleGeometry>
        {
            Values = new double[] { 3, 3, -3, -2, -4, -3, -1 },
            Fill = null
        },

        // you can also define your own SVG geometry
        new LineSeries<double, MyGeometry>
        {
            Values = new double[] { -2, 2, 1, 3, -1, 4, 3 },

            Stroke = new SolidColorPaint(SKColors.DarkOliveGreen, 3),
            Fill = null,
            GeometryStroke = null,
            GeometryFill = new SolidColorPaint(SKColors.DarkOliveGreen),
            GeometrySize = 40
        }
    };
}
