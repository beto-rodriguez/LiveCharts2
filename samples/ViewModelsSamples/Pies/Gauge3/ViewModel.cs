using System;
using LiveChartsCore.Kernel;

namespace ViewModelsSamples.Pies.Gauge3;

public class ViewModel
{
    public Func<ChartPoint, string> LabelFormatter { get; set; } =
        point => $"{point.Coordinate.PrimaryValue} {point.Context.Series.Name}";
}
