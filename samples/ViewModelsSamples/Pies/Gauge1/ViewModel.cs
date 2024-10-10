using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Extensions;

namespace ViewModelsSamples.Pies.Gauge1;

public class ViewModel
{
    public IEnumerable<ISeries> Series { get; set; } =
        GaugeGenerator.BuildSolidGauge(
            new GaugeItem(
                30,          // the gauge value
                series =>    // the series style
                {
                    series.MaxRadialColumnWidth = 50;
                    series.DataLabelsSize = 50;
                }));
}
