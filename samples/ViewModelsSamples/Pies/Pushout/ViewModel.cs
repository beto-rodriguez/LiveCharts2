using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Extensions;

namespace ViewModelsSamples.Pies.Pushout;

public class ViewModel
{
    // you can convert any array, list or IEnumerable<T> to a pie series collection:
    public IEnumerable<ISeries> Series { get; set; } =
        new[] { 6, 5, 4, 3, 2 }.AsPieSeries((value, series) =>
        {
            // pushes out the slice with the value of 6 to 30 pixels.
            if (value != 6) return;

            series.Pushout = 30;
        });
}
