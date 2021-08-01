using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
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
                MaxBarWidth = double.MaxValue,
                IgnoresBarPosition = true
            },
            new ColumnSeries<int>
            {
                Values = new [] { 2, 4, 8, 9, 5, 2, 4, 7 },
                Stroke = null,
                MaxBarWidth = 30,
                IgnoresBarPosition = true
            }
        };
    }
}
