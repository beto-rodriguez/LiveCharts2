using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ViewModelsSamples
{
    public class MainVM
    {
        public ObservableCollection<ISeries<SkiaDrawingContext>> Series { get; set; }
        public List<IAxis<SkiaDrawingContext>> YAxes { get; set; }
        public List<IAxis<SkiaDrawingContext>> XAxes { get; set; }

        public MainVM()
        {
            //var animatedStrokeDash = new SolidColorPaintTask(new SKColor(217, 47, 47), 2);
            //animatedStrokeDash.SetPropertyTransition(
            //    new LiveChartsCore.Drawing.Animation(EasingFunctions.Lineal, TimeSpan.FromMilliseconds(1000), int.MaxValue),
            //    nameof(animatedStrokeDash.PathEffect));
            //animatedStrokeDash.PathEffect = new DashPathEffect(new[] { 5f, 5f}, 0);
            //animatedStrokeDash.PathEffect = new DashPathEffect(new[] { 5f, 5f}, 10);

            //var animatedGradient = new SolidColorPaintTask(new SKColor(2, 136, 209, 200));
            //animatedGradient.SetPropertyTransition(
            //   new LiveChartsCore.Drawing.Animation(EasingFunctions.Lineal, TimeSpan.FromMilliseconds(1000), int.MaxValue),
            //   nameof(animatedGradient.Shader));
            //animatedGradient.Shader = new LinearGradientShader(
            //    new SKPoint(0, 0),
            //    new SKPoint(100, 100),
            //    new SKColor[] { new SKColor(2, 136, 209), SKColors.WhiteSmoke, new SKColor(2, 136, 209) },
            //    new[] { 0f, 0.5f, 1},
            //    SKShaderTileMode.Repeat);
            //animatedGradient.Shader = new LinearGradientShader(
            //    new SKPoint(0, 0),
            //    new SKPoint(100, 100),
            //    new SKColor[] { SKColors.WhiteSmoke, new SKColor(2, 136, 209), SKColors.WhiteSmoke },
            //    new[] { 0f, 0.5f, 1 },
            //    SKShaderTileMode.Repeat);

            Series = new ObservableCollection<ISeries<SkiaDrawingContext>>
            {
                //new StackedColumnSeries<double>
                //{
                //    Name = "columns",
                //    Values = new[]{ 2d, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2},
                //    Stroke = new SolidColorPaintTask(new SKColor(217, 47, 47), 2),
                //    Fill = new SolidColorPaintTask(new SKColor(217, 47, 47, 30)),
                //    HighlightFill = new SolidColorPaintTask(new SKColor(217, 47, 47, 80))
                //},
                //new StackedColumnSeries<double>
                //{
                //    Name = "columns 2",
                //    Values = new[]{ 2d, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2},
                //    Stroke = new SolidColorPaintTask(new SKColor(2, 136, 209), 2),
                //    Fill = new SolidColorPaintTask(new SKColor(2, 136, 209, 30)),
                //    HighlightFill = new SolidColorPaintTask(new SKColor(217, 47, 47, 80)),
                //},
                //new StackedColumnSeries<double>
                //{
                //    Name = "columns 3",
                //    Values = new[]{ 2d, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2},
                //    Stroke = new SolidColorPaintTask(new SKColor(67, 160, 61), 2),
                //    Fill = new SolidColorPaintTask(new SKColor(67, 160, 61, 30)),
                //    HighlightFill = new SolidColorPaintTask(new SKColor(217, 47, 47, 80))
                //},
                new ScatterSeries<double>
                {
                    Name = "scatter",
                    Values = new[]{ 2d, 4, 3, 1, 8, 3, 7, 2, 6, 3, 7, 3},
                    Stroke = new SolidColorPaintTask(new SKColor(239, 108, 0), 2),
                    Fill = new SolidColorPaintTask(new SKColor(239, 108, 0, 30)),
                },
                // new LineSeries<double>
                //{
                //    Name = "lines",
                //    Values = new[]{ 1d, 4, 2, 1, 7, 3, 5, 6, 3, 6, 8, 3},
                //    Stroke = new SolidColorPaintTask(new SKColor(2, 136, 209), 3),
                //    Fill = new SolidColorPaintTask(new SKColor(2, 136, 209, 200)),
                //    ShapesFill = new SolidColorPaintTask(new SKColor(255, 255, 255)),
                //    ShapesStroke =  new SolidColorPaintTask(new SKColor(2, 136, 209), 3),
                //    HighlightFill = new SolidColorPaintTask(new SKColor(2, 136, 209), 3),
                //    HighlightStroke = new SolidColorPaintTask(new SKColor(20, 20, 20), 3)
                //},
            };

            YAxes = new List<IAxis<SkiaDrawingContext>>
            {
                new Axis
                {
                    TextBrush = new TextPaintTask(new SKColor(90,90,90), 25),
                    SeparatorsBrush = new SolidColorPaintTask(new SKColor(180, 180, 180)),
                    LabelsRotation = 0
                }
            };

            XAxes = new List<IAxis<SkiaDrawingContext>>
            {
                new Axis
                {
                    TextBrush = new TextPaintTask(new SKColor(90,90,90), 25),
                    SeparatorsBrush = new SolidColorPaintTask(new SKColor(180, 180, 180)),
                    LabelsRotation = 0,
                    Labeler = (value, tick) => $"this {value}"
                }
            };
        }
    }

    public class HelloGeometry : SVGPathGeometry
    {
        // This SVG path was taken from MS docs
        // https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/curves/path-data

        private static readonly SKPath helloPath = SKPath.ParseSvgPathData(
                "M 0 0 L 0 100 M 0 50 L 50 50 M 50 0 L 50 100" +                // H
                "M 125 0 C 60 -10, 60 60, 125 50, 60 40, 60 110, 125 100" +     // E
                "M 150 0 L 150 100, 200 100" +                                  // L
                "M 225 0 L 225 100, 275 100" +                                  // L
                "M 300 50 A 25 50 0 1 0 300 49.9 Z");                           // O

        public HelloGeometry()
            : base(helloPath) // We pass the already parsed SVG path, this way it is not parsed for every shape.
        {
            // alternatively we could use the SVG property.
            // but then the SVGPathGeometryClass would require to parse the SVG for each instance.
            // SVG = "M 0 0 L 0 100 M 0 50 L 50 50 M 50 0 L 50 100" +                // H
            //       "M 125 0 C 60 -10, 60 60, 125 50, 60 40, 60 110, 125 100" +     // E
            //       "M 150 0 L 150 100, 200 100" +                                  // L
            //       "M 225 0 L 225 100, 275 100" +                                  // L
            //       "M 300 50 A 25 50 0 1 0 300 49.9 Z";                            // O
        }
    }

    public class HelloColumnSeries : ColumnSeries<double, HelloGeometry>
    {

    }
}
