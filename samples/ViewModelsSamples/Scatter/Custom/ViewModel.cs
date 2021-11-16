using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ViewModelsSamples.Scatter.Custom
{
    public class ViewModel
    {
        public ViewModel()
        {
            var r = new Random();
            var values1 = new ObservableCollection<ObservablePoint>();
            var values2 = new ObservableCollection<ObservablePoint>();

            for (var i = 0; i < 20; i++)
            {
                values1.Add(new ObservablePoint(r.Next(0, 20), r.Next(0, 20)));
                values2.Add(new ObservablePoint(r.Next(0, 20), r.Next(0, 20)));
            }

            Series = new ObservableCollection<ISeries>
            {
                // use the second type argument to specify the geometry to draw for every point
                // there are already many predefined geometries in the
                // LiveChartsCore.SkiaSharpView.Drawing.Geometries namespace
                new ScatterSeries<ObservablePoint, RoundedRectangleGeometry>
                {
                    Values = values1,
                    Stroke = null,
                    GeometrySize = 40,
                },

                // Or Define your own SVG geometry
                new ScatterSeries<ObservablePoint, MyGeometry>
                {
                    Values = values2,
                    GeometrySize = 40,
                    Stroke = null,
                    Fill = new SolidColorPaint(SKColors.DarkOliveGreen)
                }
            };
        }

        public IEnumerable<ISeries> Series { get; set; }
    }

    public class MyGeometry : SVGPathGeometry
    {
        // the static field is important to prevent the svg path is parsed multiple times // mark
        // Icon from Google Material Icons font.
        // https://fonts.google.com/icons?selected=Material%20Icons%20Outlined%3Amy_location%3A
        public static SKPath svgPath = SKPath.ParseSvgPathData(
            "M12 8c-2.21 0-4 1.79-4 4s1.79 4 4 4 4-1.79 4-4-1.79-4-4-4zm8.94 3c-.46-4.17-3.77-7.48-7.94-7.94V1h-2v2.06C6.83 3.52 3.52 6.83 3.06 " +
            "11H1v2h2.06c.46 4.17 3.77 7.48 7.94 7.94V23h2v-2.06c4.17-.46 7.48-3.77 7.94-7.94H23v-2h-2.06zM12 19c-3.87 0-7-3.13-7-7s3.13-7 7-7 7 " +
            "3.13 7 7-3.13 7-7 7z");

        public MyGeometry()
            : base(svgPath)
        {

        }
    }
}
