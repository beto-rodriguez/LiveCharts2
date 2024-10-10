using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.General.NullPoints;

public class ViewModel
{
    public ISeries[] Series { get; set; } = [
        new ColumnSeries<double?>
        {
            Values = [5, 4, null, 3, 2, 6, 5, 6, 2]
        },
        new LineSeries<double?>
        {
            Values = [2, 6, 5, 3, null, 5, 2, 4, null]
        },
        new LineSeries<ObservablePoint?>
        {
            Values = [
                new ObservablePoint { X = 0, Y = 1 },
                new ObservablePoint { X = 1, Y = 4 },
                null,
                new ObservablePoint { X = 4, Y = 5 },
                new ObservablePoint { X = 6, Y = 1 },
                new ObservablePoint { X = 8, Y = 6 },
            ]
        }
    ];
}
