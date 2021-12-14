using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Pies.Doughnut;

public class ViewModel
{
    public IEnumerable<ISeries> Series { get; set; } = new List<ISeries>
        {
            new PieSeries<double> { Values = new List<double> { 2 }, InnerRadius = 50 },
            new PieSeries<double> { Values = new List<double> { 4 }, InnerRadius = 50 },
            new PieSeries<double> { Values = new List<double> { 1 }, InnerRadius = 50 },
            new PieSeries<double> { Values = new List<double> { 4 }, InnerRadius = 50 },
            new PieSeries<double> { Values = new List<double> { 3 }, InnerRadius = 50 }
        };
}
