using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;

namespace ViewModelsSamples.StepLines.Area
{
    public class ViewModel
    {
        public List<ISeries> Series { get; set; } = new List<ISeries>
        {
            new StepLineSeries<double>
            {
                Values = new List<double> { -2, -1, 3, 5, 3, 4, 6 },
                // Set he Fill property to build an area series
                // by default the series has a fill color based on your app theme
                Fill = new SolidColorPaintTask(SKColors.CornflowerBlue),

                Stroke = null,
                GeometryFill = null,
                GeometryStroke = null
            }
        };

        // Creates a gray background and border in the draw margin.
        public DrawMarginFrame DrawMarginFrame => new DrawMarginFrame
        {
            Fill = new SolidColorPaintTask(new SKColor(220, 220, 220)),
            Stroke = new SolidColorPaintTask(new SKColor(180, 180, 180), 2)
        };
    }
}
