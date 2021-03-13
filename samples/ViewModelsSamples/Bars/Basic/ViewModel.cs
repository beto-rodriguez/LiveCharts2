using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;

namespace ViewModelsSamples.Bars.Basic
{
    public class ViewModel
    {
        public List<ISeries> Series { get; set; } = new List<ISeries>
        {
            new ColumnSeries<double>
            {
                Values = new List<double> { 2, 5, 4, -2, 4, -3, 5 }
            }
        };
    }
}
