using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;

namespace ViewModelsSamples.Lines.Basic
{
    public class ViewModel
    {
        public List<ISeries> Series { get; set; } = new List<ISeries>
        {
            new LineSeries<double>
            {
                Values = new List<double> { -2, -1, 3, 5, 3, 4, 6 }
            }
        };
    }
}
