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
            // use the second type argument to specify the geometry to draw for every point
            // there are already many predefined geometries in the
            // LiveChartsCore.SkiaSharpView.Drawing.Geometries namespace
            new ColumnSeries<double, LiveChartsCore.SkiaSharpView.Drawing.Geometries.OvalGeometry>
            {
                Values = new List<double> { 4, 2, 0, 5, 2, 6 },
                Fill = new SolidColorPaint(SKColors.CornflowerBlue)
            },

            // you can also define your own geometry using SVG
            new ColumnSeries<double, MyGeometry>
            {
                Values = new List<double> { 3, 2, 3, 4, 5, 3 },
                Stroke = new SolidColorPaint(SKColors.Coral, 1),
                Fill = null
            }
        };
    }

    public class MyGeometry : LiveChartsCore.SkiaSharpView.Drawing.Geometries.SVGPathGeometry
    {
        // the static field is important to prevent the svg path is parsed multiple times // mark
        // Icon from Google Material Icons font.
        // https://fonts.google.com/icons?selected=Material%20Icons%20Outlined%3Avertical_align_top%3A
        public static SKPath svgPath = SKPath.ParseSvgPathData(
            "M8 11h3v10h2V11h3l-4-4-4 4zM4 3v2h16V3H4z");

        public MyGeometry()
            : base(svgPath)
        {

        }
    }
}
