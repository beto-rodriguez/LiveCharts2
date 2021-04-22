using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;

namespace ViewModelsSamples.Bars.Layered
{
    public class ViewModel
    {
        public List<ISeries> Series { get; set; } = new List<ISeries>
        {
            new ColumnSeries<int>
            {
                Values = new [] { 6, 3, 5, 7, 3, 4, 6, 3 },
                Stroke = null,
                Fill = new SolidColorPaintTask(SKColors.BlueViolet),
                MaxBarWidth = double.MaxValue,
                IgnoresBarPosition = true
            },
            new ColumnSeries<int>
            {
                Values = new [] { 2, 4, 8, 9, 5, 2, 4, 7 },
                Stroke = null,
                Fill = new SolidColorPaintTask(SKColors.CornflowerBlue),
                MaxBarWidth = 30,
                IgnoresBarPosition = true
            }
        };
    }
}
