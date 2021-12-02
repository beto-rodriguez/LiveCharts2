using SkiaSharp;

namespace ViewModelsSamples.Bars.Custom
{
    public class MyGeometry : LiveChartsCore.SkiaSharpView.Drawing.Geometries.SVGPathGeometry
    {
        // the static field is important to prevent the svg path is parsed multiple times // mark
        // Icon from Google Material Icons font.
        // https://fonts.google.com/icons?selected=Material%20Icons%20Outlined%3Avertical_align_top%3A
        public static SKPath svgPath = SKPath.ParseSvgPathData(
            "M8 11h3v10h2V11h3l-4-4-4 4zM4 3v2h16V3H4z");

        public MyGeometry()
            : base(svgPath)
        { }
    }
}
