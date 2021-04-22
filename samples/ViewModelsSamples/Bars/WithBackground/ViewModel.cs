using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;

namespace ViewModelsSamples.Bars.WithBackground
{
    public class ViewModel
    {
        public List<ISeries> Series { get; set; } = new List<ISeries>
        {
            new ColumnSeries<double>
            {
                Values = new List<double> { 10, 10, 10, 10, 10, 10, 10 },
                Stroke = null,
                Fill = new SolidColorPaintTask(new SKColor(30, 30, 30, 30)),
                IgnoresBarPosition = true
            },
            new ColumnSeries<double>
            {
                Values = new List<double> { 3, 10, 5, 3, 7, 3, 8 },
                Stroke = null,
                Fill = new SolidColorPaintTask(SKColors.CornflowerBlue),
                IgnoresBarPosition = true
            }
        };
    }
}
