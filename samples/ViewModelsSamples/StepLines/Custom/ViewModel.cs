using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.StepLines.Custom;

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
            new StepLineSeries<double, MyGeometry>
            {
                Values = new List<double> { -2, 2, 1, 3, -1, 4, 3 },

                Stroke = new SolidColorPaint(SKColors.DarkOliveGreen, 3),
                Fill = null,
                GeometryStroke = null,
                GeometryFill = new SolidColorPaint(SKColors.DarkOliveGreen),
                GeometrySize = 40
            }
        };
}
