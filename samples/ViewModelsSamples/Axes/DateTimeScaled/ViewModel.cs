using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Axes.DateTimeScaled;

public partial class ViewModel : ObservableObject
{
    public ISeries[] Series { get; set; } =
    {
        new ColumnSeries<DateTimePoint>
        {
            YToolTipLabelFormatter = (chartPoint) =>
                $"{new DateTime((long) chartPoint.Coordinate.SecondaryValue):MMMM dd}: {chartPoint.Coordinate.PrimaryValue}",
            Values = new ObservableCollection<DateTimePoint>
            {
                new DateTimePoint(new DateTime(2021, 1, 1), 3),
                // notice we are missing the day new DateTime(2021, 1, 2)
                new DateTimePoint(new DateTime(2021, 1, 3), 6),
                new DateTimePoint(new DateTime(2021, 1, 4), 5),
                new DateTimePoint(new DateTime(2021, 1, 5), 3),
                new DateTimePoint(new DateTime(2021, 1, 6), 5),
                new DateTimePoint(new DateTime(2021, 1, 7), 8),
                new DateTimePoint(new DateTime(2021, 1, 8), 6)
            }
        }
    };

    public Axis[] XAxes { get; set; } =
    {
        new Axis
        {
            Labeler = value => new DateTime((long) value).ToString("MMMM dd"),
            LabelsRotation = 80,

            // when using a date time type, let the library know your unit // mark
            UnitWidth = TimeSpan.FromDays(1).Ticks, // mark

            // if the difference between our points is in hours then we would:
            // UnitWidth = TimeSpan.FromHours(1).Ticks,

            // since all the months and years have a different number of days
            // we can use the average, it would not cause any visible error in the user interface
            // Months: TimeSpan.FromDays(30.4375).Ticks
            // Years: TimeSpan.FromDays(365.25).Ticks

            // The MinStep property forces the separator to be greater than 1 day.
            MinStep = TimeSpan.FromDays(1).Ticks // mark
        }
    };
}
