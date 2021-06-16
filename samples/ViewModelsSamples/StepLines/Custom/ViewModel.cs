using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;

namespace ViewModelsSamples.StepLines.Custom
{
    public class ViewModel
    {
        public List<ISeries> Series { get; set; } = new List<ISeries>
        {
            // use the second argument type to specify the geometry to draw for every point
            // there are already many predefined geometries in the
            // LiveChartsCore.SkiaSharpView.Drawing.Geometries namespace
            new StepLineSeries<double, LiveChartsCore.SkiaSharpView.Drawing.Geometries.RectangleGeometry>
            {
                Values = new List<double> { 3, 3, -3, -2, -4, -3, -1 },
                Fill = null,
            },

            // you can also define your own SVG geometry
            new StepLineSeries<double, MyGeomeometry>
            {
                Values = new List<double> { -2, 2, 1, 3, -1, 4, 3 },

                Stroke = new SolidColorPaintTask(SKColors.DarkOliveGreen, 3),
                Fill = null,
                GeometryStroke = null,
                GeometryFill = new SolidColorPaintTask(SKColors.DarkOliveGreen),
                GeometrySize = 40
            }
        };
    }

    public class MyGeomeometry : LiveChartsCore.SkiaSharpView.Drawing.Geometries.SVGPathGeometry
    {
        // Icon made by Freepik from www.flaticon.com
        // https://www.flaticon.com/free-icon/tick_327698?term=check&page=3&position=83&page=3&position=83&related_id=327698&origin=search
        public static SKPath svgPath = SKPath.ParseSvgPathData(
            "M256,0C114.615,0,0,114.615,0,256s114.615,256,256,256s256-114.615,256-256S397.385,0,256,0z M386.594,226.664    L252.747,360.511c-7.551,7.551-17.795,11.794-28.475,11.794s-20.923-4.243-28.475-11.795l-70.388-70.389    c-15.726-15.726-15.726-41.223,0.001-56.95c15.727-15.725,41.224-15.726,56.95,0.001l41.913,41.915l105.371-105.371    c15.727-15.726,41.223-15.726,56.951,0.001C402.319,185.44,402.319,210.938,386.594,226.664z");

        public MyGeomeometry()
            : base(svgPath)
        {
        }

        public override void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
        {
            // lets also draw a white circle as background before the svg path is drawn
            // this will just make things look better

            using (var backgroundPaint = new SKPaint())
            {
                backgroundPaint.Style = SKPaintStyle.Fill;
                backgroundPaint.Color = SKColors.White;

                var r = Width / 2;
                context.Canvas.DrawCircle(X + r, Y + r, r, backgroundPaint);
            }

            base.OnDraw(context, paint);
        }
    }
}
