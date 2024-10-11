using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Lines.XY;

public class ViewModel
{
    public ISeries[] Series { get; set; } = [
        new LineSeries<ObservablePoint>
        {
            Values = [
                new ObservablePoint(0, 4),
                new ObservablePoint(1, 3),
                new ObservablePoint(3, 8),
                new ObservablePoint(18, 6),
                new ObservablePoint(20, 12)
            ]
        }
    ];
}
