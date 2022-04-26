using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Axes.DateTimeScaled;

public class ViewModel
{
    public ViewModel()
    {
        Series = new ISeries[]
        {
            new ColumnSeries<DateTimePoint>
            {
                TooltipLabelFormatter =
                    chartPoint => $"{new DateTime((long) chartPoint.SecondaryValue):MMMM dd}: {chartPoint.PrimaryValue}",
                Values = new ObservableCollection<DateTimePoint>
                {
                    new DateTimePoint(new DateTime(2021, 1, 1), 3),
                    // ...
                    // notice we are missing the day new DateTime(2021, 1, 2)
                    // ...
                    new(new DateTime(2021, 1, 3), 6),
                    new(new DateTime(2021, 1, 4), 5),
                    new(new DateTime(2021, 1, 5), 3),
                    new(new DateTime(2021, 1, 6), 5),
                    new(new DateTime(2021, 1, 7), 8),
                    new(new DateTime(2021, 1, 8), 6)
                },
            }
        };

        // The UnitWidth is only required for column or financial series
        // because the library needs to know the width of each column, by default the
        // width is 1, but when you are using a different scale, you must let the library know it.
        XAxes = new Axis[]
        {
            new Axis
            {
                Labeler = value => new DateTime((long) value).ToString("MMMM dd"),
                LabelsRotation = 15,

                // in this case we want our columns with a width of 1 day, we can get that number
                // using the following syntax
                UnitWidth = TimeSpan.FromDays(1).Ticks, // mark

                // The MinStep property forces the separator to be greater than 1 day.
                MinStep = TimeSpan.FromDays(1).Ticks // mark

                // if the difference between our points is in hours then we would:
                // UnitWidth = TimeSpan.FromHours(1).Ticks,

                // since all the months and years have a different number of days
                // we can use the average, it would not cause any visible error in the user interface
                // Months: TimeSpan.FromDays(30.4375).Ticks
                // Years: TimeSpan.FromDays(365.25).Ticks
            }
        };
    }

    public IEnumerable<ISeries> Series { get; set; }
    public IEnumerable<ICartesianAxis> XAxes { get; set; }
}
