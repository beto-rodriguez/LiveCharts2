using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;

namespace ViewModelsSamples.Pies.Basic
{
    public class ViewModel
    {
        public IEnumerable<ISeries> Series { get; set; } = new List<ISeries>
        {
            new PieSeries<double> { Values = new List<double> { 2 } },
            new PieSeries<double> { Values = new List<double> { 4 } },
            new PieSeries<double> { Values = new List<double> { 1 } },
            new PieSeries<double> { Values = new List<double> { 4 } },
            new PieSeries<double> { Values = new List<double> { 3 } }
        };
    }
}
