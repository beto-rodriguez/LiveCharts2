using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Pies.Pushout;

public class ViewModel
{
    public IEnumerable<ISeries> Series { get; set; } = new List<ISeries>
        {
            new PieSeries<double> { Values = new List<double> { 3 }, Pushout = 4 },
            new PieSeries<double> { Values = new List<double> { 3 }, Pushout = 4 },
            new PieSeries<double> { Values = new List<double> { 3 }, Pushout = 4 },
            new PieSeries<double> { Values = new List<double> { 2 }, Pushout = 4 },
            new PieSeries<double> { Values = new List<double> { 5 }, Pushout = 30 }
        };
}
