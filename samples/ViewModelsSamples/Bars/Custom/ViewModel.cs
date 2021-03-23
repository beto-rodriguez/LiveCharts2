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
            new ColumnSeries<double, LiveChartsCore.SkiaSharpView.Drawing.Geometries.RoundedRectangleGeometry>
            {
                Values = new List<double> { 4, 2, 0, 5, 2, 6 },
                Fill = new SolidColorPaintTask(SKColors.CornflowerBlue)
            },

            // you can also define your own geometry using SVG
            new ColumnSeries<double, MyGeomeometry>
            {
                Values = new List<double> { 3, 2, 3, 4, 5, 3 },
                Stroke = new SolidColorPaintTask(SKColors.Coral, 5),
                Fill = null
            }
        };
    }

    public class MyGeomeometry : LiveChartsCore.SkiaSharpView.Drawing.Geometries.SVGPathGeometry
    {
        // Icon made by srip from www.flaticon.com
        // https://www.flaticon.com/free-icon/ruler_973004?term=ruler&related_id=973004
        public static SKPath svgPath = SKPath.ParseSvgPathData(
            "M168,0v480h144V0H168z M296,96h-56v16h56v16h-24v16h24v16h-56v16h56v16h-24v16h24v16h-56v16h56v16h-24v16h24v16h-56v16     h56v16h-24v16h24v16h-56v16h56v16h-24v16h24v16h-56v16h56v32H184V16h112V96z"
            //+ "M240,72c13.255,0,24-10.745,24-24s-10.745-24-24-24s-24,10.745-24,24S226.745,72,240,72z M240,40c4.418,0,8,3.582,8,8     s-3.582,8-8,8s-8-3.582-8-8S235.582,40,240,40z"
            );


        public MyGeomeometry()
            : base(svgPath)
        {

        }
    }
}
