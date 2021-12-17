using System;
using System.Collections.Generic;
using System.Windows.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Pies.Gauge5;

public class ViewModel
{
    private readonly Random _random = new();

    public ViewModel()
    {
        ObservableValue1 = new ObservableValue { Value = 50 };
        ObservableValue2 = new ObservableValue { Value = 80 };

        Series = new GaugeBuilder()
            .WithOffsetRadius(5)
            .WithLabelsPosition(PolarLabelsPosition.Start)
            .AddValue(ObservableValue1, "North")
            .AddValue(ObservableValue2, "South")
            .BuildSeries();
    }

    public ObservableValue ObservableValue1 { get; set; }
    public ObservableValue ObservableValue2 { get; set; }
    public IEnumerable<ISeries> Series { get; set; }
    public ICommand DoRandomChangeCommand => new Command(o => DoRandomChange());

    public void DoRandomChange()
    {
        // modifying the Value property updates and animates the chart automatically
        ObservableValue1.Value = _random.Next(0, 100);
        ObservableValue2.Value = _random.Next(0, 100);
    }
}
