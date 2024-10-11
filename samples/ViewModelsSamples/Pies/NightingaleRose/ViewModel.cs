using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Extensions;

namespace ViewModelsSamples.Pies.NightingaleRose;

public class ViewModel
{
    public IEnumerable<ISeries> Series { get; set; }

    public ViewModel()
    {
        var outer = 0;
        var data = new[] { 6, 5, 4, 3, 2 };

        // you can convert any array, list or IEnumerable<T> to a pie series collection:
        Series = data.AsPieSeries((value, series) =>
        {
            // this method is called once per element in the array
            // we are decrementing the outer radius 50px
            // on every element in the array.

            series.InnerRadius = 50;
            series.OuterRadiusOffset = outer;
            outer += 50;
        });
    }
}
