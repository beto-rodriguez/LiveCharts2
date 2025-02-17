using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.General.TemplatedLegends;

public class ViewModel
{
    public ISeries[] Series { get; set; } = [
        new ColumnSeries<double>
        {
            Name = "Roger",
            Values = [2, 1, 3, 5, 3, 4, 6]
        },
        new ColumnSeries<double>
        {
            Name = "Susan",
            Values = [1, 3, 2, 7, 3, 5, 2]
        },
    ];
}
