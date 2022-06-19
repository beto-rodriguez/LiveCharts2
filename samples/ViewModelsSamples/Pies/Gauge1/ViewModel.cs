using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ViewModelsSamples.Pies.Gauge1;

[ObservableObject]
public partial class ViewModel
{
    public IEnumerable<ISeries> Series { get; set; }
        = new GaugeBuilder()
        .WithMaxColumnWidth(30)
        .AddValue(30)
        .BuildSeries();
}
