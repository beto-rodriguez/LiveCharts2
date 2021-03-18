using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;

namespace ViewModelsSamples.Pies.NightingaleRose
{
    public class ViewModel
    {
        public IEnumerable<ISeries> Series { get; set; } = new List<ISeries>
        {
            new PieSeries<double> { Values = new List<double> { 2 }, InnerRadius = 50, MaxOuterRadius = 1.0 },
            new PieSeries<double> { Values = new List<double> { 4 }, InnerRadius = 50, MaxOuterRadius = 0.9 },
            new PieSeries<double> { Values = new List<double> { 1 }, InnerRadius = 50, MaxOuterRadius = 0.8 },
            new PieSeries<double> { Values = new List<double> { 4 }, InnerRadius = 50, MaxOuterRadius = 0.7 },
            new PieSeries<double> { Values = new List<double> { 3 }, InnerRadius = 50, MaxOuterRadius = 0.6 }
        };
    }
}
