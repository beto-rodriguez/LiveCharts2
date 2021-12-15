using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Pies.Gauge1;

public class ViewModel
{
    public IEnumerable<ISeries> Series { get; set; }
        = new GaugeBuilder()
        .WithMaxColumnWidth(30)
        .AddValue(30)
        .BuildSeries();
}
