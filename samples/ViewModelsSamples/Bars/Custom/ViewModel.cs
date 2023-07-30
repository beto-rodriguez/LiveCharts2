using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace ViewModelsSamples.Bars.Custom;

public partial class ViewModel : ObservableObject
{
    public ISeries[] Series { get; set; } =
        {
            new ColumnSeries<double>
            {
                Values = new double[] { 2, 1, 4},
            },

            // You can also use SVG paths to draw the geometry
            // LiveCharts already provides some predefined paths in the SVGPoints class.
            new ColumnSeries<double, SVGPathGeometry>
            {
                Values = new double[] { -2, 2, 1 },
                GeometrySvg = SVGPoints.Star
            },

            // you can declare your own gemetry and use the SkiaSharp api to draw it
            new ColumnSeries<double, MyGeometry>
            {
                Values = new double[] { 4, 5, 2 },
            },
        };
}
