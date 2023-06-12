using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Bars.Custom;

public partial class ViewModel : ObservableObject
{
    public ViewModel()
    {
        Series = new ISeries[]
        {
            new ColumnSeries<double>
            {
                Values = new List<double> { 4, 3, 2, 1 },
                Fill = new SolidColorPaint(SKColors.CornflowerBlue)
            },

            // use the second argument type to specify the geometry to draw for every point
            // there are already many predefined geometries in the
            // LiveChartsCore.SkiaSharpView.Drawing.Geometries namespace
            new ColumnSeries<double, OvalGeometry>
            {
                Values = new double[] { 4, 3, 2, 1 },
                Fill = new SolidColorPaint(SKColors.DarkRed)
            },

            // you can also define your own SVG geometry
            // MyGeometry class let us change the Path at runtime
            // Click on the on any point to change the path.
            // You can find the MyGeometry.cs file below
            new ColumnSeries<double, MyGeometry>
            {
                Values = new double[] { 4, 3, 2, 1 },
                Fill = new SolidColorPaint(SKColors.Black)
            }
        };

        // We can change the MyGeometry path at runtime
        // the SVGPoints class contains many predefined paths
        // MyGeometry.SelectedPath = SVGPoints.Gem;

        // You can also build your own path.
        MyGeometry.SelectedPath = SKPath.ParseSvgPathData(
           "M6 4C6 3.44772 6.44772 3 7 3H17C17.5523 3 18 3.44772 18 4V18C18 18.5523 17.5523 19 17 19H7C6.44772 " +
           "19 6 18.5523 6 18V4ZM7 1C5.34315 1 4 2.34315 4 4V20C4 21.6569 5.34315 23 7 " +
           "23H17C18.6569 23 20 21.6569 20 20V4C20 2.34315 18.6569 1 17 1H7ZM12 22C12.5523 22 13 21.5523 13 " +
           "21C13 20.4477 12.5523 20 12 20C11.4477 20 11 20.4477 11 21C11 21.5523 11.4477 22 12 22Z");
    }

    public ISeries[] Series { get; set; }
}
