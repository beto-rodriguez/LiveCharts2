using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;

namespace ViewModelsSamples.Bars.AutoUpdate
{
    public class ViewModel
    {
        public List<ISeries> Series { get; set; } = new List<ISeries>
        {
            new ColumnSeries<double>
            {
                Values = new List<double> { 2, -3, 4, 5, 5, 7}
            }
        };
    }
}
