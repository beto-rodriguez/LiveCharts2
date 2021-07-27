using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ViewModelsSamples.Axes.TimeSpanScaled
{
    public class ViewModel
    {
        public ViewModel()
        {
            Series = new ObservableCollection<ISeries>
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

            // The UnitWidth is only required for column or financial series
            // because the library needs to know the width of each column, by default the
            // width is 1, but when you are using a different scale, you must let the library know it.

            XAxes = new List<Axis>
            {
                new Axis
                {
                    Labeler = value => TimeSpan.FromTicks((long)value).ToString("fff") + " ms",

                    // in this case we want our columns with a width of 1 ms, we can get that number
                    // using the following syntax
                    UnitWidth = TimeSpan.FromMilliseconds(1).Ticks,
                    MinStep = TimeSpan.FromMilliseconds(1).Ticks,

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

        public IEnumerable<IAxis> XAxes { get; set; }
    }
}
