using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;

namespace ViewModelsSamples.Bars.Custom
{
    public class ViewModel
    {
        public List<ISeries> Series { get; set; } = new List<ISeries>
        {
            new ColumnSeries<double>
            {
                Values = new List<double> { 0, -1, 3, 5, 3, 4, 6 },

                // by default the series will have a fill and stroke
                // based on the postion of the series in the Chart.Series collection and the theme
                // but you can always set your own.

                // there are some special values you can set to DrawableTasks properties (Fill, Stroke, GeometryFill, GeometryStroke)
                // Fill = null, // setting it to null will remove the fill from the series
                // Fill = LiveChartsSkiaSharp.DefaultPaint, // using DefaultPaint will let the theme decide the color

                Stroke = null,
                Fill = new SolidColorPaintTask(SKColors.CornflowerBlue),
            },

            // use the second type argument to specify the geometry to draw for every point
            // there are already many predefined geometries in the
            // LiveChartsCore.SkiaSharpView.Drawing.Geometries namespace
            new ColumnSeries<double, LiveChartsCore.SkiaSharpView.Drawing.Geometries.RoundedRectangleGeometry>
            {
                Values = new List<double> { 3, 3, -3, -2, -4, -3, -1 },
                Fill = new SolidColorPaintTask(SKColors.Coral)
            },

            // you can also define your own geometry using SVG
            new ColumnSeries<double, MyGeomeometry>
            {
                Values = new List<double> { -2, 2, -2, 2, -2, 2, -2 },
                Stroke = new SolidColorPaintTask(SKColors.DarkOliveGreen),
            }
        };
    }

    public class MyGeomeometry : LiveChartsCore.SkiaSharpView.Drawing.Geometries.SVGPathGeometry
    {
        // Icon made by Freepik from www.flaticon.com
        // https://www.flaticon.com/free-icon/doric-column_82579?term=column&page=1&position=19&page=1&position=19&related_id=82579&origin=search
        public static SKPath svgPath = SKPath.ParseSvgPathData(
            "M328,496h-8v-24c0-4.418-3.582-8-8-8h-8c-4.411,0-8-3.589-8-8s3.589-8,8-8c4.418,0,8-3.582,8-8s-3.582-8-8-8h-0.172  l-7.652-352.009C300.512,79.897,304,76.359,304,72c0-4.418-3.582-8-8-8c-4.411,0-8-3.589-8-8s3.589-8,8-8c4.418,0,8-3.582,8-8V16h8  c4.418,0,8-3.582,8-8s-3.582-8-8-8H200c-4.418,0-8,3.582-8,8s3.582,8,8,8h8v24c0,4.418,3.582,8,8,8c4.411,0,8,3.589,8,8  s-3.589,8-8,8c-4.418,0-8,3.582-8,8c0,4.359,3.488,7.897,7.824,7.991L208.172,432H208c-4.418,0-8,3.582-8,8s3.582,8,8,8  c4.411,0,8,3.589,8,8s-3.589,8-8,8h-8c-4.418,0-8,3.582-8,8v24h-8c-4.418,0-8,3.582-8,8s3.582,8,8,8h144c4.418,0,8-3.582,8-8  S332.418,496,328,496z M288,16v16h-64V16H288z M230.624,448H248c4.418,0,8-3.582,8-8s-3.582-8-8-8h-23.824l7.652-352H256  c4.418,0,8-3.582,8-8s-3.582-8-8-8h-17.376c0.888-2.504,1.376-5.196,1.376-8s-0.488-5.496-1.376-8h34.752  c-0.888,2.504-1.376,5.196-1.376,8c0,7.11,3.111,13.505,8.04,17.904L287.824,432H280c-4.418,0-8,3.582-8,8s3.582,8,8,8h1.377  c-0.888,2.504-1.376,5.196-1.376,8s0.488,5.496,1.376,8h-50.752c0.888-2.504,1.376-5.196,1.376-8S231.512,450.504,230.624,448z   M208,480h96v16h-96V480");

        public MyGeomeometry()
            : base(svgPath)
        {

        }
    }
}
