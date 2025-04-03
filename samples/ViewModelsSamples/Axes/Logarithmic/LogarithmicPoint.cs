using System;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel;

namespace ViewModelsSamples.Axes.Logarithmic;

public class LogarithmicPoint : ChartCoordinate
{
    public LogarithmicPoint(double x, double y)
    {
        // use the yLog as the y coordinate
        var yLog = Math.Log(y, ViewModel.LogBase);
        Coordinate = new Coordinate(x, yLog);
    }
}
