using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ViewModelsSamples.Pies.Gauge3;

public partial class ViewModel : ObservableObject
{
    public IEnumerable<ISeries> Series { get; set; }
        = new GaugeBuilder()
        .WithLabelsSize(20)
        .WithLabelsPosition(PolarLabelsPosition.Start)
        .WithLabelFormatter(point => $"{point.PrimaryValue} {point.Context.Series.Name}")
        .WithInnerRadius(20)
        .WithOffsetRadius(8)
        .WithBackgroundInnerRadius(20)

        .AddValue(30, "Vanessa")
        .AddValue(50, "Charles")
        .AddValue(70, "Ana")

        .BuildSeries();
}
