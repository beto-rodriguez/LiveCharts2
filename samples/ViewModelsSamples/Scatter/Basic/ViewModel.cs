using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Scatter.Basic;

public class ViewModel
{
    public ISeries[] Series { get; set; } = [
        new ScatterSeries<ObservablePoint>
        {
            Values = [
                new(2.2, 5.4),
                new(3.6, 9.6),
                new(9.9, 5.2),
                new(8.1, 4.7),
                new(5.3, 7.1)
            ]
        }
    ];
}
