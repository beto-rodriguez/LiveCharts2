using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;

namespace ViewModelsSamples.Lines.Custom
{
    public class ViewModel
    {
        public List<ISeries> Series { get; set; } = new List<ISeries>
        {
            new LineSeries<double>
            {
                Values = new List<double> { 0, -1, 3, 5, 3, 4, 6 },

                // by default the series will have a fill and stroke
                // based on the postion of the series in the Chart.Series collection and the theme
                // but you can always set your own.

                // there are some special values you can set to DrawableTasks properties (Fill, Stroke, GeometryFill, GeometryStroke)
                // Fill = null, // setting it to null will remove the fill from the series
                // Fill = LiveChartsSkiaSharp.DefaultPaint, // using DefaultPaint will let the theme decide the color

                Stroke = new SolidColorPaintTask(SKColors.CornflowerBlue) { StrokeThickness = 6 },
                Fill = LiveChartsSkiaSharp.DefaultPaint,
                GeometryStroke = new SolidColorPaintTask(SKColors.CornflowerBlue) { StrokeThickness = 6 },

                // define a custom size for each point
                GeometrySize = 20,

                // 0 straigth lines, 1 the most curved
                // out of that range will be ignored
                LineSmoothness = 0
            },

            // use the second argument type to specify the geometry to draw for every point
            // there are already many predefined geometries in the
            // LiveChartsCore.SkiaSharpView.Drawing.Geometries namespace
            new LineSeries<double, LiveChartsCore.SkiaSharpView.Drawing.Geometries.RectangleGeometry>
            {
                Values = new List<double> { 3, 3, -3, -2, -4, -3, -1 },
                Fill = null,
                LineSmoothness = 1
            },

            // you can also define your own geometry using SVG
            new LineSeries<double, MyGeomeometry>
            {
                Values = new List<double> { -2, 2, -2, 2, -2, 2, -2 },

                Stroke = null,
                GeometryFill = new SolidColorPaintTask(SKColors.DarkOliveGreen),
                GeometrySize = 30,

                // the pivot defines where the fill center is, default is 0
                Pivot = 1
            }
        };
    }

    public class MyGeomeometry : LiveChartsCore.SkiaSharpView.Drawing.Geometries.SVGPathGeometry
    {
        // Icon made by Freepik from www.flaticon.com
        // https://www.flaticon.com/free-icon/happy_1023656?term=happy&page=1&position=3&page=1&position=3&related_id=1023656&origin=search
        public static SKPath svgPath = SKPath.ParseSvgPathData(
            "M437.019,74.981C388.668,26.629,324.38,0,256,0S123.332,26.629,74.981,74.981C26.628,123.332,0,187.62,0,256    s26.628,132.668,74.981,181.019C123.332,485.371,187.62,512,256,512s132.668-26.629,181.019-74.981    C485.372,388.668,512,324.38,512,256S485.372,123.332,437.019,74.981z M256,481.524c-124.354,0-225.524-101.17-225.524-225.524    S131.646,30.476,256,30.476S481.524,131.646,481.524,256S380.354,481.524,256,481.524z" +
            "M200.622,188.396c-24.953-24.955-65.556-24.953-90.509,0c-5.951,5.95-5.951,15.599,0,21.55    c5.952,5.95,15.601,5.95,21.551,0c13.072-13.071,34.34-13.07,47.41,0c2.976,2.976,6.875,4.464,10.774,4.464    s7.8-1.488,10.774-4.464C206.573,203.995,206.573,194.347,200.622,188.396z" +
            "M401.884,188.396c-24.953-24.953-65.556-24.955-90.509,0c-5.951,5.95-5.951,15.599,0,21.55    c5.952,5.95,15.601,5.95,21.551,0c13.07-13.071,34.338-13.072,47.41,0c2.976,2.976,6.875,4.464,10.774,4.464    s7.8-1.488,10.774-4.464C407.835,203.995,407.835,194.347,401.884,188.396z" +
            "M391.111,267.175H120.889c-8.416,0-15.238,6.823-15.238,15.238c0,82.902,67.447,150.349,150.349,150.349    s150.349-67.447,150.349-150.349C406.349,273.997,399.527,267.175,391.111,267.175z M256,402.286    c-60.938,0-111.402-45.703-118.909-104.635H374.91C367.402,356.583,316.938,402.286,256,402.286z");

        public MyGeomeometry()
            : base(svgPath)
        {

        }
    }
}
