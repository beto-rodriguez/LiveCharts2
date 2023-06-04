using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
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

            // you can also define your own SVG geometry
            // MyGeometry class let us change the Path at runtime
            // Click on the on any point to change the path.
            // You can find the MyGeometry.cs file below
            new LineSeries<double, MyGeometry>
            {
                Values = new double[] { -2, 2, 1, 3, -1, 4, 3 },

                Stroke = new SolidColorPaint(SKColors.DarkOliveGreen, 3),
                Fill = null,
                GeometryStroke = new SolidColorPaint(SKColors.DarkOliveGreen, 3),
                GeometryFill = new SolidColorPaint(SKColors.White),
                GeometrySize = 35
            }
        };

        // We can change the MyGeometry path at runtime
        // the SVGPoints class contains many predefined paths
        MyGeometry.SelectedPath = SVGPoints.Gem;

        var variableGeometrySeries = (LineSeries<double, MyGeometry>)Series[2];

        // We toggle the geometry path on click
        variableGeometrySeries.ChartPointPointerDown +=
            (IChartView chart, ChartPoint<double, MyGeometry, LabelGeometry>? point) =>
            {
                MyGeometry.SelectedPath = MyGeometry.SelectedPath == SVGPoints.Gem
                    ? SVGPoints.Star
                    : SVGPoints.Gem;

                // call chart update to refresh the chart
                chart.CoreChart.Update();
            };

        // You can also build your own path.
        // MyGeometry.SelectedPath = SKPath.ParseSvgPathData(
        //    "M20.05 17.65C20.8054 17.0834 21.25 16.1943 21.25 15.25V4.25C21.25 " +
        //    "2.59315 19.9069 1.25 18.25 1.25H6.25C4.59315 1.25 3.25 2.59315 3.25 " +
        //    "4.25V15.25C3.25 16.1943 3.69458 17.0834 4.45 17.65L10.45 22.15C11.5167 " +
        //    "22.95 12.9833 22.95 14.05 22.15L20.05 17.65Z");
    }

    public ISeries[] Series { get; set; }
}
