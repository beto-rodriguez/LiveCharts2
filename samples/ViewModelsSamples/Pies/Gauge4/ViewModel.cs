using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Pies.Gauge4;

public class ViewModel
{
    public IEnumerable<ISeries> Series { get; set; }
        = new GaugeBuilder()
        .WithLabelsSize(20)
        .WithLabelsPosition(PolarLabelsPosition.End)
        .WithLabelFormatter(point => point.PrimaryValue.ToString())
        .WithInnerRadius(20)
        .WithMaxColumnWidth(5)
        .WithBackground(null)

        .AddValue(50, "Vanessa")
        .AddValue(80, "Charles")
        .AddValue(95, "Ana")

        .BuildSeries();
}
