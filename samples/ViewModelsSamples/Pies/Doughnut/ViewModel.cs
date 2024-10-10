using LiveChartsCore;
using System.Collections.Generic;
using LiveChartsCore.SkiaSharpView.Extensions;

namespace ViewModelsSamples.Pies.Doughnut;

public class ViewModel
{
    // you can convert any array, list or IEnumerable<T> to a pie series collection:
    public IEnumerable<ISeries> Series { get; set; } =
        new[] { 2, 4, 1, 4, 3 }.AsPieSeries((value, series) =>
        {
            series.MaxRadialColumnWidth = 60;
        });
}
