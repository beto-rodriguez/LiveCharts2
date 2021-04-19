using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;

namespace ViewModelsSamples.General.NullPoints
{
    public class ViewModel
    {
        public List<ISeries> Series { get; set; } = new List<ISeries>
        {
            // every series knows how to handle nulls.
            new ColumnSeries<double?> { Values = new List<double?> { 5, 4,  null, 3, 2, 6, 5, 6, 2 } },
            new LineSeries<double?> { Values = new List<double?> { 2, 6, 5, 3, null, 5, 2, 4, 3 } }
        };
    }
}
