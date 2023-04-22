using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ViewModelsSamples.Pies.Gauge1;

public partial class ViewModel : ObservableObject
{
    public IEnumerable<ISeries> Series { get; set; }
        = new GaugeBuilder()
        .WithMaxColumnWidth(30)
        .AddValue(30)
        .BuildSeries();
}
