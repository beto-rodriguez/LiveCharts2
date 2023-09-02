using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Axes.TimeSpanScaled;

public partial class ViewModel : ObservableObject
{
    public ISeries[] Series { get; set; } =
    {
        new ColumnSeries<TimeSpanPoint>
        {
            Values = new ObservableCollection<TimeSpanPoint>
            {
                new TimeSpanPoint(TimeSpan.FromMilliseconds(1), 10),
                new TimeSpanPoint(TimeSpan.FromMilliseconds(2), 6),
                new TimeSpanPoint(TimeSpan.FromMilliseconds(3), 3),
                new TimeSpanPoint(TimeSpan.FromMilliseconds(4), 12),
                new TimeSpanPoint(TimeSpan.FromMilliseconds(5), 8),
            },
        }
    };

    // You can use the TimeSpanAxis class to define a time span based axis // mark

    // The first parameter is the time between each point, in this case 1 day // mark
    // you can also use 1 year, 1 month, 1 hour, 1 minute, 1 second, 1 millisecond, etc // mark

    // The second parameter is a function that receives the value and returns the label // mark
    public Axis[] XAxes { get; set; } =
    {
        new TimeSpanAxis(TimeSpan.FromMilliseconds(1), date => date.ToString("fff") + "ms")
    };
}
