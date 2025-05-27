using System;
using LiveChartsCore.Kernel;

namespace ViewModelsSamples.Pies.Gauge4;

public class ViewModel
{
    public Func<ChartPoint, string> Formatter { get; set; } =
        point => point.Coordinate.PrimaryValue.ToString();
}
