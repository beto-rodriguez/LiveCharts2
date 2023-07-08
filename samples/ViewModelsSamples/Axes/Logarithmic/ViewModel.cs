using System;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Axes.Logarithmic;

public partial class ViewModel : ObservableObject
{
    // base 10 log, change the base if you require it.
    // or use any custom scale the logic is the same.
    private static readonly int s_logBase = 10;

    public ISeries[] Series { get; set; } =
    {
        new LineSeries<LogarithmicPoint>
        {
            Mapping = (logPoint, chartPoint) =>
            {
                // for the x coordinate, we use the X property
                // and for the Y coordinate, we will map it to the logarithm of the value
                chartPoint.Coordinate = new(logPoint.X, Math.Log(logPoint.Y, s_logBase));
            },
            Values = new LogarithmicPoint[]
            {
                new() { X = 1, Y = 10 },
                new() { X = 2, Y = 100 },
                new() { X = 3, Y = 1000 },
                new() { X = 4, Y = 10000 },
                new() { X = 5, Y = 100000 },
                new() { X = 6, Y = 1000000 },
                new() { X = 7, Y = 10000000 }
            }
        }
    };

    public Axis[] YAxes { get; set; } =
    {
        new Axis
        {
            // forces the step of the axis to be at least 1
            MinStep = 1,

            // converts the log scale back for the label
            Labeler = value => Math.Pow(s_logBase, value).ToString() // mark
        }
    };
}
