using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;

namespace ViewModelsSamples.Lines.Straight
{
    public class ViewModel
    {
        public List<ISeries> Series { get; set; } = new List<ISeries>
        {
            new LineSeries<double>
            {
                Values = new List<double> { -2, -1, 3, 5, 3, 4, 6 },
                Fill = null,
                // use the line smoothness property to control the curve
                // it goes from 0 to 1
                // where 0 is a straight line and 1 the most curved
                LineSmoothness = 0
            }
        };
    }
}
