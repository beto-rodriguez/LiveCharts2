using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;

namespace ViewModelsSamples.Lines.Area
{
    public class ViewModel
    {
        public List<ISeries> Series { get; set; } = new List<ISeries>
        {
            new LineSeries<double>
            {
                Values = new List<double> { -2, -1, 3, 5, 3, 4, 6 },
                // Set he Fill property to build an area series
                // by defualt the series has a fill color based on your app theme
                Fill = new SolidColorPaintTask(SKColors.CornflowerBlue),

                Stroke = null,
                GeometryFill = null,
                GeometryStroke = null
            }
        };
    }
}
